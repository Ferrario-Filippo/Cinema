namespace Cinema.Helpers
{
	public static class FileHelpers
	{
		public static void DeleteImageIfExists(string rootPath, string path)
		{
			var oldImagePath = Path.Combine(
					rootPath,
					path.TrimStart(Path.DirectorySeparatorChar));

			if (System.IO.File.Exists(oldImagePath))
				System.IO.File.Delete(oldImagePath);
		}
	}
}
