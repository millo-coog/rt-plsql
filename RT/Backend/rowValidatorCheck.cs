using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RT {
	public class rowValidatorCheck {
		private string prvFieldName;
		private string prvFieldValue;
		private string prvComparisonType;

		// Get/set methods
		public string fieldName {
			get { return prvFieldName; }
			set { prvFieldName = value; }
		}
		public string fieldValue{
			get { return prvFieldValue; }
			set { prvFieldValue = value; }
		}
		public string comparisonType{
			get { return prvComparisonType; }
			set { prvComparisonType = value; }
		}

		// Constructor
		public rowValidatorCheck() { }
	}

	// Declare a collection iterator for our row validator check class
	public class rowValidatorCheckCollection : System.ComponentModel.BindingList<rowValidatorCheck> {
		// A method for creating a complete clone of the row validation checks in this collection
		public rowValidatorCheckCollection clone() {
			rowValidatorCheckCollection newCollection = new rowValidatorCheckCollection();

			foreach (rowValidatorCheck currRowValidator in this) {
				rowValidatorCheck newCheck = new rowValidatorCheck();

				newCheck.comparisonType = currRowValidator.comparisonType;
				newCheck.fieldName = currRowValidator.fieldName;
				newCheck.fieldValue = currRowValidator.fieldValue;

				newCollection.Add(newCheck);
			}

			return newCollection;
		}

		public rowValidatorCheck this[String fieldName] {
			get {
				foreach (rowValidatorCheck currRowValidator in this) {
					if (currRowValidator.fieldName == fieldName) {
						return currRowValidator;
					}
				}

				return null;
			}
		}

		public void Remove(String fieldName) {
			foreach (rowValidatorCheck currRowValidator in this) {
				if (currRowValidator.fieldName == fieldName) {
					this.Remove(currRowValidator);
					break;
				}
			}
		}
	}
}
