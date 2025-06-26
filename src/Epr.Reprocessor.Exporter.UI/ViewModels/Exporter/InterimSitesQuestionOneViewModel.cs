namespace Epr.Reprocessor.Exporter.UI.ViewModels.Reprocessor;
using System.ComponentModel.DataAnnotations;
using Resources.Views.Exporter;

public class InterimSitesQuestionOneViewModel
{
    [Required(ErrorMessageResourceType = typeof(InterimSitesQuestionOne), ErrorMessageResourceName = "Please_select_an_option")]
    public bool HasInterimSites { get; set; }
}