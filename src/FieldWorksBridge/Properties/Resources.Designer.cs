﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4952
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FieldWorksBridge.Properties {
	using System;


	/// <summary>
	///   A strongly-typed resource class, for looking up localized strings, etc.
	/// </summary>
	// This class was auto-generated by the StronglyTypedResourceBuilder
	// class via a tool like ResGen or Visual Studio.
	// To add or remove a member, edit your .ResX file then rerun ResGen
	// with the /str option, or rebuild your VS project.
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
	[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
	[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	internal class Resources {

		private static global::System.Resources.ResourceManager resourceMan;

		private static global::System.Globalization.CultureInfo resourceCulture;

		[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Resources() {
		}

		/// <summary>
		///   Returns the cached ResourceManager instance used by this class.
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Resources.ResourceManager ResourceManager {
			get {
				if (object.ReferenceEquals(resourceMan, null)) {
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FieldWorksBridge.Properties.Resources", typeof(Resources).Assembly);
					resourceMan = temp;
				}
				return resourceMan;
			}
		}

		/// <summary>
		///   Overrides the current thread's CurrentUICulture property for all
		///   resource lookups using this strongly typed resource class.
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Globalization.CultureInfo Culture {
			get {
				return resourceCulture;
			}
			set {
				resourceCulture = value;
			}
		}

		internal static System.Drawing.Icon chorus {
			get {
				object obj = ResourceManager.GetObject("chorus", resourceCulture);
				return ((System.Drawing.Icon)(obj));
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Select a Chorus-enabled FieldWorks project to open:.
		/// </summary>
		internal static string kChorusEnabledFwProject {
			get {
				return ResourceManager.GetString("kChorusEnabledFwProject", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Cannot clone onto an existing directory ({0}).
		/// </summary>
		internal static string kCloneTrouble {
			get {
				return ResourceManager.GetString("kCloneTrouble", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to FieldWorks Bridge.
		/// </summary>
		internal static string kFieldWorksBridge {
			get {
				return ResourceManager.GetString("kFieldWorksBridge", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to FieldWorks Project File (*.fwdata)|*.fwdata.
		/// </summary>
		internal static string kFwDataFileFilter {
			get {
				return ResourceManager.GetString("kFwDataFileFilter", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Locate FieldWorks XML Data File.
		/// </summary>
		internal static string kLocateFwDataFile {
			get {
				return ResourceManager.GetString("kLocateFwDataFile", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Lock file present.
		/// </summary>
		internal static string kLockFilePresent {
			get {
				return ResourceManager.GetString("kLockFilePresent", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to It appears that FLEx is running, since the file: {0}.fwdata.lock is present. You need to close FLEx or delete that file, if FLEx is not running, and try again..
		/// </summary>
		internal static string kLockFilePresentMsg {
			get {
				return ResourceManager.GetString("kLockFilePresentMsg", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to No paths given.
		/// </summary>
		internal static string kNoPathsGiven {
			get {
				return ResourceManager.GetString("kNoPathsGiven", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Not a FieldWorks XML file..
		/// </summary>
		internal static string kNotAnFwXmlFile {
			get {
				return ResourceManager.GetString("kNotAnFwXmlFile", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Select the new folder where the shared data goes..
		/// </summary>
		internal static string kSelectClonedDataFolder {
			get {
				return ResourceManager.GetString("kSelectClonedDataFolder", resourceCulture);
			}
		}
	}
}