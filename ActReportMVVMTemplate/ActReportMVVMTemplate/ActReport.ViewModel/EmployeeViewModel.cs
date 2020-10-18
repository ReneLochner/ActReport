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
        private Employee _selectedEmployee;
        private ObservableCollection<Employee> _employees;
        private ICommand _cmdSaveChanges;

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
                        canExecute: _ => _selectedEmployee != null);
                }

                return _cmdSaveChanges;
            }
            set {
                _cmdSaveChanges = value;
            }
        }

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
    }
}
