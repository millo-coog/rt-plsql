using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Oracle.DataAccess.Client;
using System.Diagnostics;

namespace RT {
	public class testArgument {
		#region Get/set methods
		// Remember to add any new ones to the clone!
		public string argumentName { get; set; }
		public string inOut { get; set; }
		public int sequence { get; set;  }
		public int dataLevel { get; set; }
		public int position { get; set; }
		public string dataType { get; set; }
		public string plsType { get; set; }
		public bool canDefault { get; set; }
		public testArgumentCollection childArguments { get; set; }
		#endregion

		// Constructors
		public testArgument() {
			childArguments = new testArgumentCollection();
		}

		public testArgument(OracleDataReader rdrArgs) {
			this.argumentName = rdrArgs["argument_name"].ToString();
			this.inOut = rdrArgs["in_out"].ToString();
			this.dataType = rdrArgs["data_type"].ToString();
			this.plsType = rdrArgs["pls_type"].ToString();

			this.sequence = Int32.Parse(rdrArgs["sequence"].ToString());
			this.dataLevel = Int32.Parse(rdrArgs["data_level"].ToString());
			this.position = Int32.Parse(rdrArgs["position"].ToString());

			this.canDefault = rdrArgs["defaulted"].ToString() == "Y";
			
			childArguments = new testArgumentCollection();
		}

		public bool isReturnArgument() {
			return (inOut == "RETURN");
		}

		// Clones the current argument.
		public testArgument clone() {
			testArgument newArg = new testArgument();

			newArg.argumentName = this.argumentName;
			newArg.canDefault = this.canDefault;
			newArg.inOut = this.inOut;
			newArg.dataType = this.dataType;
			newArg.plsType = this.plsType;

			newArg.sequence = this.sequence;
			newArg.dataLevel = this.dataLevel;
			newArg.position = this.position;

			if (childArguments.Count > 0)
				newArg.childArguments = this.childArguments.clone();

			return newArg;
		}

		public string getPLSQLTypeDeclaration() {
			string plsql;

			switch (this.plsType) {
				case "":
					if (this.inOut == "RETURN")
						plsql = "t_ReturnValueRec";
					else {
						Debug.Assert(this.dataType == "PL/SQL RECORD");

						plsql = "t_Rec" + this.sequence.ToString() + "$";
					}

					break;

				case "RAW":
				case "VARCHAR2":
				case "NVARCHAR2":
					plsql = this.plsType + "(32767)";
					break;

				default:
					plsql = this.plsType;
					break;
			}

			return plsql;
		}
	}

	// Declare a collection iterator for our test argument class
	public class testArgumentCollection : List<testArgument> {
		// Constructor
		public testArgumentCollection() { }

		// Finds the given parameter in the collection by name
		public testArgument this[String argumentName] {
			get {
				foreach (testArgument arg in this) {
					if (arg.argumentName == argumentName) {
						return arg;
					}
				}

				return null;
			}
			set {
				for (int i = 0; i < this.Count; i++) {
					if (this[i].argumentName == argumentName) {
						this[i] = value;
						break;
					}
				}
			}
		}

		// Clones the current argument collection.
		public testArgumentCollection clone() {
			testArgumentCollection newArgCollection = new testArgumentCollection();

			foreach (testArgument currArg in this) {
				newArgCollection.Add(currArg.clone());
			}

			return newArgCollection;
		}

		// Renames (overlays) the given old argument with the given new argument in the collection...
		public void renameArgument(testArgument oldArg, testArgument newArg) {
			foreach (testArgument arg in this) {
				if (arg == oldArg) {
					this[arg.argumentName] = newArg;
					break;
				} else {
					if (arg.childArguments.Count > 0) {
						arg.childArguments.renameArgument(oldArg: oldArg, newArg: newArg);
					}
				}
			}
		}

		// Removes the given argument from the collection, even if nested
		public void removeArgument(testArgument argToRemove) {
			foreach (testArgument arg in this) {
				if (arg == argToRemove) {
					this.Remove(item: arg);
					break;
				} else {
					if (arg.childArguments.Count > 0) {
						arg.childArguments.removeArgument(argToRemove: argToRemove);
					}
				}
			}
		}
	}
}
