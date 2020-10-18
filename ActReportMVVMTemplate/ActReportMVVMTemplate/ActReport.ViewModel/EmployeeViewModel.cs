using ActReport.Core.Contracts;
using ActReport.Core.Entities;
using ActReport.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ActReport.ViewModel {
    public class EmployeeViewModel : BaseViewModel {
        private string _firstName;
        private string _lastName;
        private string _filterString;
        private Employee _newEmployee;
        private Employee _selectedEmployee;
        private ObservableCollection<Employee> _employees;
        private ICommand _cmdSaveChanges;
        private ICommand _cmdAddEntry;
        private ICommand _cmdFilterEmployees;

        public EmployeeViewModel()
        {
            LoadEmployees();
        }

        public string FirstName {
            get => _firstName;
            set {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        public string LastName {
            get => _lastName;
            set {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        public string FilterString {
            get => _filterString;
            set {
                _filterString = value;
                OnPropertyChanged(nameof(FilterString));
            }
        }

        public Employee SelectedEmployee {
            get => _selectedEmployee;
            set {
                _selectedEmployee = value;
                FirstName = _selectedEmployee?.FirstName;
                LastName = SelectedEmployee?.LastName;
                OnPropertyChanged(nameof(SelectedEmployee));
            }
        }

        public ObservableCollection<Employee> Employees {
            get => _employees;
            set {
                _employees = value;
                OnPropertyChanged(nameof(Employees));
            }
        }

        private void LoadEmployees()
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                var employees = uow.EmployeeRepository
                    .Get(
                        orderBy:
                            coll => coll.OrderBy(emp => emp.LastName)
                    )
                    .ToList();

                Employees = new ObservableCollection<Employee>(employees);
            }
        }

        public ICommand CmdSaveChanges {
             get {
                if (_cmdSaveChanges == null)
                {
                    _cmdSaveChanges = new RelayCommand(
                        execute: _ =>
                        {
                            using IUnitOfWork uow = new UnitOfWork();
                            _selectedEmployee.FirstName = _firstName;
                            _selectedEmployee.LastName = _lastName;
                            uow.EmployeeRepository.Update(_selectedEmployee);
                            uow.Save();

                            LoadEmployees();
                        },
                        canExecute: _ => _selectedEmployee != null && LastName.Length >= 3);
                }

                return _cmdSaveChanges;
            }
            set {
                _cmdSaveChanges = value;
            }
        }

        public ICommand CmdAddEntry {
            get {
                if (_cmdAddEntry == null)
                {
                    _cmdAddEntry = new RelayCommand(
                        execute: _ =>
                        {
                            using IUnitOfWork uow = new UnitOfWork();
                            _newEmployee = new Employee()
                            {
                                FirstName = _firstName,
                                LastName = _lastName
                            };
                            uow.EmployeeRepository.Insert(_selectedEmployee);
                            uow.Save();

                            LoadEmployees();
                        },
                        canExecute: _ => 
                        !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName));
                }

                return _cmdAddEntry;
            }
            set {
                _cmdAddEntry = value;
            }
        }

        public ICommand CmdFilterEmployees {
            get {
                throw new NotImplementedException();
            }
        }
    }
}
