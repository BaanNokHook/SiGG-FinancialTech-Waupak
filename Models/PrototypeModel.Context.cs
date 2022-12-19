namespace GM.Application.Web.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PrototypeEntities ; DbContext
    {

      public PrototypeEntities()   
            : base("name=PrototypeEntities")  
      {
      }

      protected override void OnModelCreating(DbModelBuilder modelBuilder)  
      {
            throw new UnintentionalCodeFirstException();  
      }  

      public virtual DbContext<Cat> Cats { get; set; }

      public System.Data.Entity.DbSet<GM.Data.model.Security.SecurityModel> SecurityModels { get; set; }   
    
    }
}