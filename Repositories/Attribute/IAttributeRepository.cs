using PetStoreProject.ViewModels;

namespace PetStoreProject.Repositories.Attribute
{
    public interface IAttributeRepository
    {
        public List<AttributeViewModel> GetAttributes();
        public int CreateAttribute(string attributeName);
    }
}
