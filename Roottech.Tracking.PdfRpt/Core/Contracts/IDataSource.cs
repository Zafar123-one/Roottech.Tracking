﻿using System.Collections.Generic;

namespace Roottech.Tracking.PdfRpt.Core.Contracts
{
	/// <summary>
	/// PdfRpt's DataSource Contract
	/// </summary>
	public interface IDataSource
	{
		/// <summary>
		/// The data to render.
		/// </summary>
		/// <returns></returns>
		IEnumerable<IList<CellData>> Rows();
	}
}
