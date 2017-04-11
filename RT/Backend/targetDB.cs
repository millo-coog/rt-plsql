/*
 * This class holds methods relating to working with the target database.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Diagnostics;
using System.Data;
using System.Windows.Forms;

namespace RT {
	public class targetDB {
		#region Properties
		public string excludedSchemas { get; set; }
		public string name { get; set; }
		public OracleConnection conTargetDB = new OracleConnection();
		#endregion

		#region Methods
		// Opens the targetDB's database connection
		public void open(string username, string password) {
			if (conTargetDB.State != ConnectionState.Open) {
				conTargetDB.ConnectionString = "User Id=" + username + ";Password=" + password + ";Data Source=" + name;
				
				conTargetDB.Open();
			}
		}
		
		// Returns the list of objects in the given schema of the given type
		public List<targetDBObject> getObjects(string schema, string objectType) {
			List<targetDBObject> lstDBObjects = new List<targetDBObject>();

			OracleCommand cmdGetObjects = new OracleCommand();
			cmdGetObjects.Connection = this.conTargetDB;

			cmdGetObjects.CommandText = @"
				SELECT all_objects.object_name
				  FROM sys.all_objects
				 WHERE all_objects.owner = :p_Schema
				   AND all_objects.object_type = :p_ObjectType
				   AND NOT all_objects.object_name LIKE 'SYS_PLSQL_%'
				 ORDER BY all_objects.object_name";

			cmdGetObjects.Parameters.Add("p_Schema", schema);
			cmdGetObjects.Parameters.Add("p_ObjectType", objectType);
			
			OracleDataReader drObjects = cmdGetObjects.ExecuteReader();

			// Add the objects to our list
			while (drObjects.Read()) {
				targetDBObject newObject = new targetDBObject();

				newObject.schema = schema;
				newObject.name = drObjects["object_name"].ToString();
				newObject.objectType = objectType;

				lstDBObjects.Add(newObject);
			}

			drObjects.Close();
			drObjects.Dispose();

			cmdGetObjects.Dispose();

			return lstDBObjects;
		}

		// Returns the list of all methods in the target DB.
		public List<targetDBMethod> allMethods(string schema, string objectType, string name) {
			List<targetDBMethod> lstAllMethods = new List<targetDBMethod>();
			
			OracleCommand cmdGetMethods = new OracleCommand(@"
				SELECT all_procedures.procedure_name, NVL(all_procedures.overload, 0) AS overload
				  FROM sys.all_procedures
				 WHERE all_procedures.owner = :p_Schema
				   AND all_procedures.object_type = :p_ObjectType
				   AND all_procedures.object_name = :p_Name
				   AND all_procedures.subprogram_id != 0
				 ORDER BY all_procedures.procedure_name, TO_NUMBER(all_procedures.overload) NULLS FIRST");

			cmdGetMethods.Parameters.Add("p_Schema", schema);
			cmdGetMethods.Parameters.Add("p_ObjectType", objectType);
			cmdGetMethods.Parameters.Add("p_Name", name);

			cmdGetMethods.Connection = this.conTargetDB;

			OracleDataReader drMethods = cmdGetMethods.ExecuteReader();
			
			while (drMethods.Read()) {
				targetDBMethod newMethod = new targetDBMethod();

				newMethod.schema = schema;
				newMethod.name = name;
				newMethod.method = drMethods["procedure_name"].ToString();
				newMethod.overload = Int32.Parse(drMethods["overload"].ToString());
				newMethod.objectType = objectType;

				lstAllMethods.Add(newMethod);
			}

			drMethods.Close();
			drMethods.Dispose();

			cmdGetMethods.Dispose();

			return lstAllMethods;
		}
		
		public static testArgumentCollection getMethodArguments(OracleConnection conTargetDB, string schema, string package, string method, int overload, bool returnedAsNested = true) {
			OracleDataReader rdrArgs;

			OracleCommand cmdGetArgumentInfo = new OracleCommand(@"
				SELECT all_arguments.sequence, all_arguments.data_level,
				       LEAD(data_level) OVER (ORDER BY SEQUENCE) AS next_data_level,
				       all_arguments.position, all_arguments.data_type, 
				       CASE
				       WHEN all_arguments.position = 0 THEN 'v_returnvalue'
				       ELSE NVL(LOWER(all_arguments.argument_name), '$$' || LAG(SUBSTR(argument_name, 1, 28)) OVER (ORDER BY sequence))
				       END AS argument_name,
						 DECODE(all_arguments.position, 0, 'RETURN', all_arguments.in_out) AS in_out, all_arguments.defaulted,
						 NVL(all_arguments.pls_type, DECODE(all_arguments.data_type, 'REF CURSOR', 'SYS_REFCURSOR', RTRIM(type_owner || '.' || type_name || '.' || type_subname, '.'))) AS pls_type
				  FROM sys.all_arguments
				 WHERE all_arguments.owner = :p_Schema
					AND (all_arguments.package_name = :p_Package OR (:p_Package IS NULL AND all_arguments.package_name IS NULL))
					AND all_arguments.object_name = :p_Method
					AND NVL(all_arguments.overload, 0) = :p_Overload
					AND NOT (all_arguments.position = 1 AND NVL(all_arguments.data_type, ' ') = 'OBJECT' AND NVL(all_arguments.argument_name, ' ') = 'SELF' AND NVL(all_arguments.type_owner, ' ') = owner AND all_arguments.type_name = NVL(all_arguments.package_name, ' ')) -- Ignore the implicit SELF first argument to type's member methods
				 ORDER BY all_arguments.sequence",
				conTargetDB);

			cmdGetArgumentInfo.BindByName = true;
			cmdGetArgumentInfo.Parameters.Add("p_Schema", OracleDbType.Varchar2, schema, ParameterDirection.Input);
			cmdGetArgumentInfo.Parameters.Add("p_Package", OracleDbType.Varchar2, package, ParameterDirection.Input);
			cmdGetArgumentInfo.Parameters.Add("p_Method", OracleDbType.Varchar2, method, ParameterDirection.Input);
			cmdGetArgumentInfo.Parameters.Add("p_Overload", OracleDbType.Int32, overload, ParameterDirection.Input);

			rdrArgs = cmdGetArgumentInfo.ExecuteReader();

			cmdGetArgumentInfo.Dispose();

			testArgumentCollection testArgs = new testArgumentCollection();
			testArgumentCollection returnValue = new testArgumentCollection();

			if (returnedAsNested) {
				if (rdrArgs.Read()) {
					testArgs = readChildArgs(rdrArgs: rdrArgs, data_level: 0);
				}
			} else {
				testArgument newArg;

				if (rdrArgs.Read()) {
					if (Int32.Parse(rdrArgs["position"].ToString()) == 0) {
						// This is a function - read all records that describe the return value...
						returnValue.Add(new testArgument(rdrArgs: rdrArgs));

						while (rdrArgs.Read()) {
							newArg = new testArgument(rdrArgs: rdrArgs);

							if (Int32.Parse(rdrArgs["data_level"].ToString()) == 0) {
								// We've read past the return value arguments...
								testArgs.Add(newArg);
								break;
							} else {
								returnValue.Add(newArg);
							}
						}

						foreach (testArgument retArg in returnValue) {
							retArg.inOut = "RETURN";
						}
					} else {
						testArgs.Add(new testArgument(rdrArgs: rdrArgs));
					}

					// Read any non-return value arguments
					while (rdrArgs.Read()) {
						testArgs.Add(new testArgument(rdrArgs: rdrArgs));
					}

					// Add the return value arguments to the end of the argument collection
					testArgs.AddRange(returnValue);
				}
			}

			rdrArgs.Close();
			rdrArgs.Dispose();

			return testArgs;
		}

		private static testArgumentCollection readChildArgs(OracleDataReader rdrArgs, int data_level, bool isReturnArg = false) {
			testArgumentCollection testArgs = new testArgumentCollection();

			testArgument returnValue = null;

			while (true) {
				testArgument newArg = new testArgument(rdrArgs: rdrArgs);

				if (isReturnArg || Int32.Parse(rdrArgs["position"].ToString()) == 0)
					newArg.inOut = "RETURN";

				if (rdrArgs["data_type"].ToString() == "TABLE"
					|| rdrArgs["data_type"].ToString() == "PL/SQL TABLE"
					|| rdrArgs["data_type"].ToString() == "PL/SQL RECORD"
					|| (rdrArgs["data_type"].ToString() == "REF CURSOR" && rdrArgs["next_data_level"].ToString() != "" && Int32.Parse(rdrArgs["next_data_level"].ToString()) == data_level+1))
				{
					rdrArgs.Read();
					newArg.childArguments = readChildArgs(rdrArgs: rdrArgs, data_level: data_level + 1, isReturnArg: newArg.inOut == "RETURN" || isReturnArg);
				}

				if (newArg.position == 0) {
					// Save the return value arg for the end of the collection
					returnValue = newArg;
				} else {
					testArgs.Add(newArg);
				}

				if (rdrArgs["next_data_level"] == null) {
					break;
				} else {
					if (rdrArgs["next_data_level"].ToString() != "" && Int32.Parse(rdrArgs["next_data_level"].ToString()) == data_level) {
						if (rdrArgs.Read() == false) {
							break;
						}
					} else {
						break;
					}
				}
			}

			if (returnValue != null)
				testArgs.Add(returnValue);

			return testArgs;
		}
		#endregion
	}

	// Holds information about a targetable database object.
	public class targetDBObject {
		public string schema;
		public string name;
		public string objectType;
	}

	// Holds information about a targetable method.
	public class targetDBMethod {
		public string schema;
		public string name;
		public string method;
		public int overload;
		public string objectType;
	}
}
