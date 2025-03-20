using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Roottech.Tracking.PdfRpt.Core.Contracts;

namespace Roottech.Tracking.RTM.UI.Web.Areas.Stationary.Controllers
{
    public class CustomSum : IAggregateFunction
    {
        const string Pattern = @"<td\b[^>]*?>(?<V>[\s\S]*?)</\s*td>";
        private static readonly Regex ValueFormatMatch = new Regex(Pattern, RegexOptions.Compiled);

        string _groupSum = "0:0:0";
        string _overallSum = "0:0:0";
        /// <summary>
        /// Fires before rendering of this cell.
        /// Now you have time to manipulate the received object and apply your custom formatting function.
        /// It can be null.
        /// </summary>
        public Func<object, string> DisplayFormatFormula { set; get; }

        /// <summary>
        /// Returns current groups' aggregate value.
        /// </summary>
        public object GroupValue => _groupSum;

        /// <summary>
        /// Returns current row's aggregate value without considering the presence of the groups.
        /// </summary>
        public object OverallValue => _overallSum;

        /// <summary>
        /// Fires after adding a cell to the main table.
        /// </summary>
        /// <param name="cellDataValue">Current cell's data</param>
        /// <param name="isNewGroupStarted">Indicated starting a new group</param>
        public void CellAdded(object cellDataValue, bool isNewGroupStarted)
        {
            checkNewGroupStarted(isNewGroupStarted);

            if (cellDataValue == null) return;

            var html = cellDataValue.ToString();
            var values = ValueFormatMatch.Matches(html).Cast<Match>()
                .Select(match => match.Groups["V"].Value)
                .ToList();
            //var val = float.Parse(values.Last());
            //_groupSum += val;
            //_overallSum += val;

            var hour = Convert.ToInt16(short.Parse(html.Split(':')[0]) + short.Parse(_groupSum.Split(':')[0]));
            var min = Convert.ToInt16(short.Parse(html.Split(':')[1]) + short.Parse(_groupSum.Split(':')[1]));
            var sec = Convert.ToInt16(short.Parse(html.Split(':')[2]) + short.Parse(_groupSum.Split(':')[2]));

            _overallSum = _groupSum = GetTimeInFullByHourAndMinuteAndSecond(hour, min, sec);
        }

        /// <summary>
        /// A general method which takes a list of data and calculates its corresponding aggregate value.
        /// It will be used to calculate the aggregate value of each pages individually, with considering the previous pages data.
        /// </summary>
        /// <param name="columnCellsSummaryData">List of data</param>
        /// <returns>Aggregate value</returns>
        public object ProcessingBoundary(IList<SummaryCellData> columnCellsSummaryData)
        {
            if (columnCellsSummaryData == null || !columnCellsSummaryData.Any()) return 0;

            var list = columnCellsSummaryData;

            //float sum = 0;
            Int16 hour = 0;
            Int16 min = 0;
            Int16 sec = 0;
            foreach (var item in list)
            {
                if (item.CellData.PropertyValue == null) continue;

                var html = item.CellData.PropertyValue.ToString();
                //var values = ValueFormatMatch.Matches(html).Cast<Match>()
                //    .Select(match => match.Groups["V"].Value)
                //    .ToList();
                //var val = float.Parse(values.Last());

                hour += Convert.ToInt16(short.Parse(html.Split(':')[0]));
                min += Convert.ToInt16(short.Parse(html.Split(':')[1]));
                sec += Convert.ToInt16(short.Parse(html.Split(':')[2]));
                
                //sum += val;
            }

            return GetTimeInFullByHourAndMinuteAndSecond(hour, min, sec); //sum;
        }

        private void checkNewGroupStarted(bool newGroupStarted)
        {
            if (newGroupStarted) _groupSum = "";
        }


        private string GetTimeInFullByHourAndMinuteAndSecond(Int16 hour, Int16 minute, Int16 second)
        {
            var addMinutes = Math.Floor(second / 60.0);
            second = Convert.ToInt16(second - (60 * Math.Floor(second / 60.0)));
            minute += Convert.ToInt16(addMinutes);

            var addHours = Math.Floor(minute / 60.0);
            minute = Convert.ToInt16(minute - (60 * Math.Floor(minute / 60.0)));
            hour += Convert.ToInt16(addHours);

            return $"{hour}:{minute}:{second}";
        }
    }
}