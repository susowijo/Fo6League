//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Fo6League.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class publicite
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public publicite()
        {
            this.Pictures = new HashSet<Picture>();
        }
    
        public int ID { get; set; }
        public string Text { get; set; }
        public int LeagueID { get; set; }
        public byte[] Video { get; set; }
        public string VideoMIME { get; set; }
    
        public virtual League League { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Picture> Pictures { get; set; }
    }
}
