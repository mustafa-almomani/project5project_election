//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace project_election.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class GeneralListing
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GeneralListing()
        {
            this.GeneralListCandidates = new HashSet<GeneralListCandidate>();
        }
    
        public int GeneralListingID { get; set; }
        public string Name { get; set; }
        public Nullable<int> NumberOfVotes { get; set; }
        public bool Sucess { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GeneralListCandidate> GeneralListCandidates { get; set; }
    }
}
