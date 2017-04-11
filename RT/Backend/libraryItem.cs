/*
 * This class represents a library item - a stored item of text that a test can refer to.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RT {
	public class libraryItem {
		// Private variables
		private string prvName;
		private string prvDescription;
		private string prvValue;

		// Properties
		public string name {
			get { return prvName; }
			set { prvName = value; }
		}
		public string description {
			get { return prvDescription; }
			set { prvDescription = value; }
		}
		public string value {
			get { return prvValue; }
			set { prvValue = value; }
		}

		// Clones the current library item...
		public libraryItem clone() {
			libraryItem newItem = new libraryItem();

			newItem.name = this.prvName;
			newItem.description = this.prvDescription;
			newItem.value = this.prvValue;

			return newItem;
		}
	}

	// Declare a collection iterator for our library item class
	public class libraryItemCollection : System.Collections.Generic.List<libraryItem> {
		// Clones every library item in the entire collection to a new collection.
		public libraryItemCollection clone() {
			libraryItemCollection newCollection = new libraryItemCollection();

			foreach (libraryItem currLibraryItem in this) {
				newCollection.Add(currLibraryItem.clone());
			}

			return newCollection;
		}

		// Provides a name indexer, to find elements...
		public libraryItem this[String itemName] {
			get {
				foreach (libraryItem currLibraryItem in this) {
					if (currLibraryItem.name == itemName) {
						return currLibraryItem;
					}
				}

				return null;
			}
		}

		// Removes the given library item from the collection...
		public void Remove(String itemName) {
			foreach (libraryItem currLibraryItem in this) {
				if (currLibraryItem.name == itemName) {
					this.Remove(currLibraryItem);
					break;
				}
			}
		}
	}
}
