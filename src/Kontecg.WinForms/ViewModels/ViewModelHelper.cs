using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using System;
using System.ComponentModel;
using Kontecg.Extensions;
using Kontecg.Localization;
using DevExpress.Mvvm.Native;

namespace Kontecg.ViewModels
{
    public static class ViewModelHelper
    {
        public static Type GetUnproxiedType(this object proxyInstance)
        {
            return ViewModelSourceHelper.IsPOCOGeneratedType(proxyInstance.GetType()) ? proxyInstance.GetType().BaseType : proxyInstance.GetType();
        }

        public static TViewModel GetParentViewModel<TViewModel>(object viewModel)
            where TViewModel : class
        {
            if (viewModel is ISupportParentViewModel parentViewModelSupport)
                return parentViewModelSupport.ParentViewModel as TViewModel;
            return null;
        }

        public static void EnsureModuleViewModel(object view, object parentViewModel, object parameter = null)
        {
            if (view is ISupportViewModel vm)
            {
                KontecgViewModelBase oldViewModel = null;
                if(vm.ViewModel is ISupportParentViewModel parentViewModelSupport)
                    oldViewModel = parentViewModelSupport.ParentViewModel as KontecgViewModelBase;

                EnsureViewModel(vm.ViewModel, parentViewModel, parameter);

                if (oldViewModel != parentViewModel)
                    vm.ParentViewModelAttached();
            }
        }

        public static void EnsureViewModel(object viewModel, object parentViewModel, object parameter = null)
        {
            if (viewModel is ISupportParentViewModel parentViewModelSupport)
                parentViewModelSupport.ParentViewModel = parentViewModel;

            if(viewModel is ISupportParameter parameterSupport && parameter != null)
                parameterSupport.Parameter = parameter;
        }

        public static void RaiseCanExecuteChanged(object viewModel, string methodName)
        {
            var viewModelType = viewModel.GetType();
            var commandMethod = viewModelType.GetMethod(methodName);
            if (commandMethod != null)
            {
                var commandProperty = viewModelType.GetProperty(string.Concat(methodName, "Command"));
                if (commandProperty != null)
                {
                    var commandObj = commandProperty.GetValue(viewModel, null) as IDelegateCommand;
                    commandObj?.RaiseCanExecuteChanged();
                }
            }
        }

        public static string GetEntityDisplayName(Type type)
        {
            KontecgDisplayNameAttribute attribute = (KontecgDisplayNameAttribute) AttributesHelper.GetAttributes(type)[typeof(KontecgDisplayNameAttribute)];
            return attribute == null || attribute == DisplayNameAttribute.Default ? type.Name.RemovePostFix("Dto") : attribute.DisplayName;
        }
    }
}
