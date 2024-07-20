using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SapmleApplication.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SapmleApplication.Pages
{
    public class SequenceAdapterParamsModel : PageModel
    {
        [BindProperty]
        public BindParams? Input { get; set; }

        public readonly List<SelectListItem> Units = new List<SelectListItem>
        {
            new SelectListItem("sec","1000",true),
            new SelectListItem("msec","1")
        };

        public void OnGet(String mode)
        {
        }

        public void OnPost(String mode) 
        {
        }

        public List<String>? CustomDisplayValidationErrors()
        {
            if(ModelState.IsValid) {
                return null;
            }
            else {
                List<String> result = new List<string>();
                Regex index_pattern = new Regex(@".*\[(\d+)\].*");
                foreach(var kvp in ModelState) {
                    String prefix = "";
                    Match match= index_pattern.Match(kvp.Key);
                    if(match.Success) prefix=match.Groups[1].Value+": ";
                    if(kvp.Value.ValidationState==ModelValidationState.Invalid) {
                        foreach(var error in kvp.Value.Errors) {
                            result.Add(prefix+error.ErrorMessage);
                        }
                        
                    }
                }
                return result;
            }

        }

        public class BindParams
        {
            public List<BindStage>? Stages { get; set; }
        }

        public class BindStage
        {
            [Required]
            public Int32? Count { get; set; }
            [Required]
            public Int32? Delay { get; set; }
            [DisplayName("Units")]
            public Int32 Scale { get; set; }
        }

    }
}
