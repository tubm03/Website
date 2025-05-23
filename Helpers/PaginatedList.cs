namespace PetStoreProject.Helpers
{
	public class PaginatedList<T> : List<T>
	{
		public int pageIndex;
		public int totalPage;

		public bool hasPreviousPage => pageIndex > 1;
		public bool hasNextPage => pageIndex < totalPage;

		public PaginatedList(List<T> items, int itemsSize, int pageNumber, int pageSize)
		{
			pageIndex = pageNumber;
			totalPage = (int)Math.Ceiling((double)itemsSize / pageSize);
			this.AddRange(items);
		}

		public static PaginatedList<T> Create(List<T> listT, int pageNumber, int pageSize)
		{
			int sourceSize = listT.Count;
			var items = listT.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
			PaginatedList<T> paginatedList = new PaginatedList<T>(items, sourceSize, pageNumber, pageSize);
			return paginatedList;
		}
	}
}
