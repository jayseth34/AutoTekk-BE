using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DL;
using WebApplication1.Models;

namespace WebApplication1.BL
{
	public class ItemBL
	{
		private readonly IConfiguration config;
		private readonly string dbConn;
		public ItemBL(IConfiguration _config)
		{
			config = _config;
			dbConn = config.GetValue<string>("ConnectionStrings");
		}

		public async Task<ItemRs> AddItem(ItemRq oitemRq)
		{
			ItemRs oitemRs = new ItemRs();
			ItemDL itemDL = new ItemDL(this.config);
			if (oitemRq.isitemupdate)
			{
				oitemRs = itemDL.UpdateItem(oitemRq);
			} 
			else
			{
				oitemRs = itemDL.AddItem(oitemRq);
			}
			
			return oitemRs;
		}

		public async Task<GetItemListRs> GetItemList(Int64 registeredphonenumber)
		{
			GetItemListRs oGetItemListRs = new GetItemListRs();
			ItemDL itemDL = new ItemDL(this.config);
			oGetItemListRs = itemDL.GetItemList(registeredphonenumber);
			return oGetItemListRs;
		}

		public async Task<GetItemRs> GetItemDetails(Int64 registeredphonenumber, string itemname)
		{
			GetItemRs oGetItemRs = new GetItemRs();
			ItemDL itemDL = new ItemDL(this.config);
			oGetItemRs = itemDL.GetItemDetails(registeredphonenumber, itemname);
			return oGetItemRs;
		}

		public async Task<GetCategoryRs> GetCategory(Int64 registeredphonenumber)
		{
			GetCategoryRs oGetCategoryRs = new GetCategoryRs();
			ItemDL itemDL = new ItemDL(this.config);
			oGetCategoryRs = itemDL.GetCategory(registeredphonenumber);
			return oGetCategoryRs;
		}

		public async Task<GetItemByCategoryRs> GetItemByCategory(Int64 registeredphonenumber, string category)
		{
			GetItemByCategoryRs oGetItemByCategoryRs = new GetItemByCategoryRs();
			ItemDL itemDL = new ItemDL(this.config);
			oGetItemByCategoryRs = itemDL.GetItemByCategory(registeredphonenumber, category);
			return oGetItemByCategoryRs;
		}
		public async Task<AddUpdateCategoryRs> AddUpdateCategory(AddUpdateCategoryRq oAddUpdateCategoryRq)
		{
			AddUpdateCategoryRs oAddUpdateCategoryRs = new AddUpdateCategoryRs();
			ItemDL itemDL = new ItemDL(this.config);
			oAddUpdateCategoryRs = itemDL.AddUpdateCategory(oAddUpdateCategoryRq);
			return oAddUpdateCategoryRs;
		}

		public async Task<AssignCodeRs> AssignCode(AssignCodeRq oAssignCodeRq)
		{
			AssignCodeRs oAssignCodeRs = new AssignCodeRs();
			ItemDL itemDL = new ItemDL(this.config);
			oAssignCodeRs.status = "FAILED";
			long value = await itemDL.GetNextSequenceValue();
			if(value != 0)
			{
				oAssignCodeRs.status = "SUCCESS";
				oAssignCodeRs.assignedcode = value;
			}
			return oAssignCodeRs;
		}

		public async Task<List<ItemRq>> ReadAndMapExcelAsync(IFormFile file, Int64 registeredphonenumber)
		{
			var itemList = new List<ItemRq>();

			try
			{
				using (var stream = new MemoryStream())
				{
					await file.CopyToAsync(stream);
					using (var workbook = new XLWorkbook(stream))
					{
						// Check if the workbook has at least one worksheet
						if (workbook.Worksheets.Count == 0)
						{
							throw new Exception("The Excel file does not contain any worksheets.");
						}

						// Get the first worksheet (index 1 is valid in this case)
						var worksheet = workbook.Worksheet(1);
						if (worksheet == null)
						{
							throw new Exception("The specified worksheet could not be found.");
						}

						var rows = worksheet.RowsUsed().Skip(1); // Skip header row
						Console.WriteLine($"Rows found: {rows.Count()}");

						foreach (var row in rows)
						{
							try
							{
								// Debugging: Log row data
								Console.WriteLine($"Processing row {row.RowNumber()}: {string.Join(", ", row.Cells().Select(c => c.GetString()))}");

								var itemName = row.Cell("A").GetString();
								if (string.IsNullOrEmpty(itemName))
								{
									Console.WriteLine($"Item name is empty in row {row.RowNumber()}");
									continue; // Skip empty rows
								}

								var item = new ItemRq
								{
									itemname = itemName,
									itemcode = row.Cell("B").GetString(),
									category = row.Cell("C").GetString() ?? "GENERAL",
									itemhsn = row.Cell("D").GetString(),
									mrp = GetDecimalValue(row.Cell("E")),
									saleprice = GetDecimalValue(row.Cell("F")),
									purchaseprice = GetDecimalValue(row.Cell("G")),
									wholesaleprice = GetDecimalValue(row.Cell("H")),
									minimumwholesalequantity = row.Cell("I").GetValue<int>(),
									openingquantity = GetDecimalValue(row.Cell("J")),
									minimumstocktomaintain = GetDecimalValue(row.Cell("K")),
									_location = row.Cell("L").GetString() ?? "",
									registeredphonenumber = registeredphonenumber,
									remainingquantity = GetDecimalValue(row.Cell("J")),
								};

								itemList.Add(item);
							}
							catch (Exception ex)
							{
								// Log the error for the specific row (optional)
								Console.WriteLine($"Error processing row {row.RowNumber()}: {ex.Message}");
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				// Log the exception or handle it appropriately
				throw new Exception("Error processing the Excel file: " + ex.Message);
			}

			Console.WriteLine($"Total records processed: {itemList.Count}");
			return itemList;
		}

		private decimal GetDecimalValue(IXLCell cell)
		{
			// Check if the cell is empty or contains invalid data
			if (cell == null || string.IsNullOrWhiteSpace(cell.GetString()))
			{
				return 0; // Return 0 if the cell is empty or null
			}

			decimal value = 0;
			if (decimal.TryParse(cell.GetString(), out value))
			{
				return value;
			}

			// Log or handle the invalid value
			Console.WriteLine($"Invalid decimal value in cell: {cell.Address}");
			return 0; // Return 0 if the value cannot be parsed as decimal
		}



		public async Task<ItemRs> ProcessItemsAsync(List<ItemRq> items)
		{
			ItemRs oitemRs = new ItemRs();

			foreach (var item in items)
			{
				ItemDL itemDL = new ItemDL(this.config);
				oitemRs = itemDL.AddItem(item);
			}

			return oitemRs;
		}
	}
}
