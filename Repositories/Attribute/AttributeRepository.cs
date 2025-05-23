using PetStoreProject.Models;
using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Attribute
{
    public class AttributeRepository : IAttributeRepository
    {
        private readonly PetStoreDBContext _context;

        public AttributeRepository(PetStoreDBContext context)
        {
            _context = context;
        }

        public int CreateAttribute(string attributeName)
        {
            var attribute = new Models.Attribute
            {
                Name = attributeName
            };
            _context.Attributes.Add(attribute);
            _context.SaveChanges();
            return attribute.AttributeId;
        }

        public List<AttributeViewModel> GetAttributes()
        {
            var attributes = _context.Attributes.Select(a => new AttributeViewModel
            {
                Id = a.AttributeId,
                Name = a.Name
            }).ToList();

            return attributes;
        }
    }
}
