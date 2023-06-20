/*
 * Created by SharpDevelop.
 * User: zack
 * Date: 2019/01/09
 * Time: 13:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using rokugaTouroku.info;

namespace rokugaTouroku
{
	/// <summary>
	/// Description of sortableList.
	/// </summary>
	public class SortableBindingList<T> : BindingList<T> where T : class
    {
        private bool _isSorted;
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;
        private PropertyDescriptor _sortProperty;
        
        private string columnName = null;
        public config.config cfg = null; 
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SortableBindingList{T}"/> class.
        /// </summary>
        public SortableBindingList()
        {
        }
 
        /// <summary>
        /// Initializes a new instance of the <see cref="SortableBindingList{T}"/> class.
        /// </summary>
        /// <param name="list">An <see cref="T:System.Collections.Generic.IList`1" /> of items to be contained in the <see cref="T:System.ComponentModel.BindingList`1" />.</param>
        public SortableBindingList(IList<T> list)
            :base(list)
        {
        }
 
        /// <summary>
        /// Gets a value indicating whether the list supports sorting.
        /// </summary>
        protected override bool SupportsSortingCore
        {
            get { return true; }
        }
 
        /// <summary>
        /// Gets a value indicating whether the list is sorted.
        /// </summary>
        protected override bool IsSortedCore
        {
            get { return _isSorted; }
        }
 
        /// <summary>
        /// Gets the direction the list is sorted.
        /// </summary>
        protected override ListSortDirection SortDirectionCore
        {
            get { return _sortDirection; }
        }
 
        /// <summary>
        /// Gets the property descriptor that is used for sorting the list if sorting is implemented in a derived class; otherwise, returns null
        /// </summary>
        protected override PropertyDescriptor SortPropertyCore
        {
            get { return _sortProperty; }
        }
 
        /// <summary>
        /// Removes any sort applied with ApplySortCore if sorting is implemented
        /// </summary>
        protected override void RemoveSortCore()
        {
            _sortDirection = ListSortDirection.Ascending;
            _sortProperty = null;
            _isSorted = false; //thanks Luca
        }

        /// <summary>
        /// Sorts the items if overridden in a derived class
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="direction"></param>
        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            _sortProperty = prop;
            _sortDirection = direction;
            
            columnName = prop.Name;
            List<T> list = Items as List<T>;
            if (list == null) return;
			
            list.Sort(Compare);
 
            _isSorted = true;
            //fire an event that the list has been changed.
            //OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
 
 
        private int Compare(T lhs, T rhs)
        {
            var result = OnComparison(lhs, rhs);
            //invert if descending
            if (_sortDirection == ListSortDirection.Descending)
                result = -result;
            return result;
        }
 
        private int OnComparison(T lhs, T rhs)
        {
            object lhsValue = lhs == null ? null : _sortProperty.GetValue(lhs);
            object rhsValue = rhs == null ? null : _sortProperty.GetValue(rhs);
            if (lhsValue == null)
            {
                return (rhsValue == null) ? 0 : -1; //nulls are equal
            }
            if (rhsValue == null)
            {
                return 1; //first has value, second doesn't
            }
            
           	/*
            if (columnName == "HostId") {
            	int intL, intR;
            	var _l = int.TryParse(lhsValue.ToString(), out intL);
            	var _r = int.TryParse(rhsValue.ToString(), out intR);
            	if (_l && _r) return intL.CompareTo(intR);
            	else if (_l && !_r) return 1;
            	else if (!_l && _r) return -1;
            	return 0;
            }
            if (columnName == "elapsedTime") {
           		var lLi = (LiveInfo)(object)lhs;
           		var rLi = (LiveInfo)(object)rhs;
            	//TimeSpan intL, intR;
            	//var _l = TimeSpan.TryParse(lhsValue.ToString().Replace("日", ".").Replace("時間", ":").Replace("分", ":").Replace("秒", ""), out intL);
            	//var _r = TimeSpan.TryParse(rhsValue.ToString().Replace("日", ".").Replace("時間", ":").Replace("分", ":").Replace("秒", ""), out intR);
            	var ret = DateTime.Compare(lLi.pubDateDt, rLi.pubDateDt);
            	if (ret == 0 && lLi.lvId != rLi.lvId)
            		return string.Compare(lLi.lvId, rLi.lvId);
            	return DateTime.Compare(lLi.pubDateDt, rLi.pubDateDt);
            	
            	//if (_l && _r) return intL.CompareTo(intR);
            	//else if (_l && !_r) return 1;
            	//else if (!_l && _r) return -1;
            	//return 0;
            }
            */
            if (lhsValue is IComparable)
            {
                var ret = ((IComparable)lhsValue).CompareTo(rhsValue);
                if (ret == 0 && lhsValue is RecInfo) {
                	var lLi = (RecInfo)(object)lhs;
           			var rLi = (RecInfo)(object)rhs;
           			return string.Compare(lLi.id, rLi.id);
                }
                return ret;
            }
            if (lhsValue.Equals(rhsValue))
            {
            	var lLi = (RecInfo)(object)lhs;
           		var rLi = (RecInfo)(object)rhs;
                return string.Compare(lLi.id, rLi.id); //both are the same
            }
            //not comparable, compare ToString
            return lhsValue.ToString().CompareTo(rhsValue.ToString());
        }
    }
}
