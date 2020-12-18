using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fiorello.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Altdaki error yazini burda cixartmaq olur"),StringLength(30,ErrorMessage ="Burdada hemcinin yazmaq olar.Bir once yazdigimizi.")]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedTime { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
