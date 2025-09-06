using System.ComponentModel.DataAnnotations;
using Kontecg.ViewModels;

namespace Kontecg.Desktop.Views
{
    public class TestViewModel : KontecgViewModelBase
    {
        public TestViewModel()
        {
        }

        [StringLength(50)]
        public string TextInput { get; set; }

        public void Click()
        {
        }
    }
}