using System.Collections.Generic;
using Kontecg.Organizations.Dto;

namespace Kontecg.Views.Organizations
{
    public partial class SummaryByWorkPlaceAndCategoryDocument : DevExpress.XtraReports.UI.XtraReport
    {
        public SummaryByWorkPlaceAndCategoryDocument()
        {
            InitializeComponent();
        }

        public SummaryByWorkPlaceAndCategoryDocument(TemplateDocumentOutputDto document)
        {
            InitializeComponent();

            DataSource = new List<TemplateDocumentOutputDto>() {document};
            //xrLetterHead.ImageSource = new ImageSource(document.Company.LetterHeadFile.File);
            //xrAddress.Text = document.Company.Address;
            //xrOrganism.Text = document.Company.Organism;
            //xrCompanyName.Text = document.Company.Name;
            //xrReup.Text = document.Company.Reup;
            xrCrossTab1.DataSource = document.Template.Items;
        }
    }
}
