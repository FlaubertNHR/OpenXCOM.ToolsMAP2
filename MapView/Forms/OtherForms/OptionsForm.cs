using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Windows.Forms;

using DSShared.Windows;


namespace MapView
{
	/// <summary>
	/// The Options form.
	/// @note I doubt that a person has to emit opcodes just to write a
	/// PropertyGrid. But it's also doing a tricky conversion that alters a
	/// pen when either of its color- or width-option changes (although that
	/// could be being done in Options or Option ... cf). Unfortunately,
	/// however, TopView apparently needs to maintain 2 pens for this (one for
	/// color and one for width) yet only one is used in the actual draw-routine
	/// ofc - viz. the "color" pen not the "width" pen.
	/// - see TopView.LoadControlOptions() eg.
	/// 
	/// But even that could be done w/out emitting opcodes.
	/// </summary>
	internal sealed class OptionsForm
		:
			Form
	{
		#region cTor
		/// <summary>
		/// cTor. Constructs an OptionsForm.
		/// </summary>
		/// <param name="typeLabel"></param>
		/// <param name="options"></param>
		internal OptionsForm(string typeLabel, Options options)
		{
			InitializeComponent();

			propertyGrid.TypeLabel = typeLabel;
			propertyGrid.SetOptions(options);

			RegistryInfo.RegisterProperties(this);
		}
		#endregion cTor


		#region Events (override)
		/// <summary>
		/// Handles command-key processing. Closes this form when either of
		/// [Esc] or [Ctrl+o] is pressed.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Control | Keys.O: // non-whiteman code-page users beware ... Mista Kurtz, he dead. GLORY TO THE LOST CAUSE!!! yeah whatever.
				case Keys.Escape:
					if (!FindFocusedControl(this).GetType().ToString().Contains("GridViewEdit")) // ie. focus is not in any cell's EditMode
					{
						Close();
						return true;
					}
					break;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		#endregion (override)


		#region Events
		/// <summary>
		/// Handles this form's VisibleChanged event.
		/// @note OptionsForms are weird - although there are 4 of them, one for
		/// each viewer, there's only 1 entry in Options for all.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnVisibleChanged(object sender, EventArgs e)
		{
			if (!Visible)
				RegistryInfo.UpdateRegistry(this, true);
		}
		#endregion Events


		#region Methods (static)
		private static Control FindFocusedControl(Control control)
		{
			var container = control as ContainerControl;
			while (container != null)
			{
				control = container.ActiveControl;
				container = control as ContainerControl;
			}
			return control;
		}
		#endregion Methods (static)



		#region Designer
		private IContainer components = null;

		private OptionsPropertyGrid propertyGrid;

		/// <summary>
		/// Cleans up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();

			base.Dispose(disposing);
		}


/*		The #develop designer is going to delete this:

			this.propertyGrid = new OptionsPropertyGrid();

		Add it back to the top of InitializeComponent().
 */
		private void InitializeComponent()
		{
			this.propertyGrid = new OptionsPropertyGrid();
			this.SuspendLayout();
			// 
			// propertyGrid
			// 
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(592, 374);
			this.propertyGrid.TabIndex = 0;
			// 
			// OptionsForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(592, 374);
			this.Controls.Add(this.propertyGrid);
			this.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MinimumSize = new System.Drawing.Size(500, 300);
			this.Name = "OptionsForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Custom PropertyGrid";
			this.VisibleChanged += new System.EventHandler(this.OnVisibleChanged);
			this.ResumeLayout(false);

		}
		#endregion Designer
	}



	/// <summary>
	/// The grid in the Options form. This gets complicated.
	/// @note Definition of spaghetti-code: when it's easier to rewrite the code
	/// from scratch than figure out what's already there.
	/// </summary>
	internal sealed class OptionsPropertyGrid
		:
			PropertyGrid
	{
		#region Fields (static)
		private static Hashtable _hashTypes = new Hashtable();
		#endregion Fields (static)


		#region Fields
		private Options _options;
		private Hashtable _typeHash;
		#endregion Fields


		#region Properties
		private string _typeLabel = "DefaultType";
		[Description("Name of the type that will be internally created.")]
		internal string TypeLabel
		{
			get { return _typeLabel; }
			set { _typeLabel = value; }
		}
		#endregion Properties


		#region cTor
		internal OptionsPropertyGrid()
		{
			InitTypes();
		}
		#endregion cTor


		#region Events (override)
		// FxCop CA2123:OverrideLinkDemandsShouldBeIdenticalToBase
//		[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.LinkDemand, Name = "FullTrust")]
		protected override void OnPropertyValueChanged(PropertyValueChangedEventArgs e)
		{
			base.OnPropertyValueChanged(e);

			var option = _options[e.ChangedItem.Label] as Option;
			option.Value = e.ChangedItem.Value;
			option.doUpdate(
						e.ChangedItem.Label,
						e.ChangedItem.Value);
		}
		#endregion Events (override)


		#region Methods
		/// <summary>
		/// Initialize a private hashtable with type-opCode pairs so i dont have
		/// to write a long if/else statement when outputting MSIL.
		/// </summary>
		private void InitTypes()
		{
			_typeHash = new Hashtable();

			_typeHash[typeof(SByte)]   = OpCodes.Ldind_I1; // sbyte
			_typeHash[typeof(Byte)]    = OpCodes.Ldind_U1; // byte
			_typeHash[typeof(Char)]    = OpCodes.Ldind_U2; // char
			_typeHash[typeof(Int16)]   = OpCodes.Ldind_I2; // short
			_typeHash[typeof(UInt16)]  = OpCodes.Ldind_U2; // ushort
			_typeHash[typeof(Int32)]   = OpCodes.Ldind_I4; // int
			_typeHash[typeof(UInt32)]  = OpCodes.Ldind_U4; // uint
			_typeHash[typeof(Int64)]   = OpCodes.Ldind_I8; // long
			_typeHash[typeof(UInt64)]  = OpCodes.Ldind_I8; // ulong
			_typeHash[typeof(Boolean)] = OpCodes.Ldind_I1; // bool
			_typeHash[typeof(Double)]  = OpCodes.Ldind_R8; // double
			_typeHash[typeof(Single)]  = OpCodes.Ldind_R4; // float
		}

		/// <summary>
		/// Does some complicated stuff.
		/// </summary>
		/// <param name="options">the options to set</param>
		internal void SetOptions(Options options)
		{
			_options = options;

			// Reflection.Emit code below copied and modified
			// http://longhorn.msdn.microsoft.com/lhsdk/ref/ns/system.reflection.emit/c/propertybuilder/propertybuilder.aspx

			if (_hashTypes[TypeLabel] == null)
			{
				var ass = new AssemblyName();
				ass.Name = "TempAssembly";

				// create type
				TypeBuilder typer = Thread
									.GetDomain()
									.DefineDynamicAssembly(ass, AssemblyBuilderAccess.Run)
									.DefineDynamicModule("TempModule")
									.DefineType(TypeLabel, TypeAttributes.Public);

				// create the hashtable used to store property vals
				FieldBuilder fielder = typer.DefineField(
													"table",
													typeof(Hashtable),
													FieldAttributes.Private);
				CreateHashMethod(
							typer.DefineProperty(
											"Hash",
											PropertyAttributes.None,
											typeof(Hashtable),
											new Type[]{}),
							typer,
							fielder);

				foreach (string key in _options.Keys)
					EmitProperty(
							typer,
							fielder,
							_options[key],
							key);

				_hashTypes[TypeLabel] = typer.CreateType();
			}

			var table = new Hashtable();
			foreach (string key in _options.Keys)
				table[key] = _options[key].Value;

			var type = _hashTypes[TypeLabel] as Type;
			object o = type.GetConstructor(new Type[]{}).Invoke(new Object[]{});

			var info = type.GetProperty("Hash");	// set the object's hashtable
			info.SetValue(o, table, null);			// in the future i would like to do this in the emitted object's constructor
													// personally I'd like to do it w/out Reflection ...
			SelectedObject = o;
		}

		/// <summary>
		/// Does some more complicated stuff.
		/// </summary>
		/// <param name="proper"></param>
		/// <param name="typer"></param>
		/// <param name="info"></param>
		private static void CreateHashMethod(
				PropertyBuilder proper,
				TypeBuilder typer,
				FieldInfo info)
		{
			var getter = typer.DefineMethod( // first define the behavior of the "get" property for Hash as a method
										"GetHash",
										MethodAttributes.Public,
										typeof(Hashtable),
										new Type[]{});
			ILGenerator gen = getter.GetILGenerator();
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldfld, info);
			gen.Emit(OpCodes.Ret);

			var setter = typer.DefineMethod( // second define the behavior of the "set" property for Hash as a method
										"SetHash",
										MethodAttributes.Public,
										null,
										new []{ typeof(Hashtable) });

			gen = setter.GetILGenerator();
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldarg_1);
			gen.Emit(OpCodes.Stfld, info);
			gen.Emit(OpCodes.Ret);

			// map the two methods created above to their property
			proper.SetGetMethod(getter);
			proper.SetSetMethod(setter);

			// add the [Browsable(false)] property to the Hash property so it doesn't show up on the property list
			proper.SetCustomAttribute(
								new CustomAttributeBuilder(
														typeof(BrowsableAttribute).GetConstructor(new []{ typeof(Boolean) }),
														new object[]{ false }));
		}

		/// <summary>
		/// Emits a generic get/set property in which the result returned [there
		/// is no return..] resides in a hashtable whose key is the name of the
		/// property. Ie, does complicated stuff.
		/// </summary>
		/// <param name="typer"></param>
		/// <param name="info"></param>
		/// <param name="option"></param>
		/// <param name="label"></param>
		private void EmitProperty(
				TypeBuilder typer,
				FieldInfo info,
				Option option,
				string label)
		{
			// to figure out what opcodes to emit, i would compile a small class
			// having the functionality i wanted, and view it with ildasm.
			// peverify is also kinda nice to use to see what errors there are.

			var proper = typer.DefineProperty(
											label,
											PropertyAttributes.None,
											option.Value.GetType(),
											new Type[]{});
			var type = option.Value.GetType();
			var getter = typer.DefineMethod(
										"get_" + label,
										MethodAttributes.Public,
										type,
										new Type[]{});
			var gen = getter.GetILGenerator();
			gen.DeclareLocal(type);
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldfld, info);
			gen.Emit(OpCodes.Ldstr, label);
			gen.EmitCall(
						OpCodes.Callvirt,
						typeof(Hashtable).GetMethod("get_Item"),
						null);

			if (type.IsValueType)
			{
				gen.Emit(OpCodes.Unbox, type);
				if (_typeHash[type] != null)
					gen.Emit((OpCode)_typeHash[type]);
				else
					gen.Emit(OpCodes.Ldobj, type);
			}
			else
				gen.Emit(OpCodes.Castclass, type);

			gen.Emit(OpCodes.Stloc_0);
			gen.Emit(OpCodes.Br_S, (byte)0);
			gen.Emit(OpCodes.Ldloc_0);
			gen.Emit(OpCodes.Ret);

			var setter = typer.DefineMethod(
										"set_" + label,
										MethodAttributes.Public,
										null,
										new []{ type });
			gen = setter.GetILGenerator();
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Ldfld, info);
			gen.Emit(OpCodes.Ldstr, label);
			gen.Emit(OpCodes.Ldarg_1);

			if (type.IsValueType)
				gen.Emit(OpCodes.Box, type);

			gen.EmitCall(
						OpCodes.Callvirt,
						typeof(Hashtable).GetMethod("set_Item"),
						null);
			gen.Emit(OpCodes.Ret);

			proper.SetGetMethod(getter);
			proper.SetSetMethod(setter);

			if (option.Description != null)
				proper.SetCustomAttribute(
									new CustomAttributeBuilder(
															typeof(DescriptionAttribute).GetConstructor(new []{ typeof(String) }),
															new object[]{ option.Description }));

			if (option.Category != null)
				proper.SetCustomAttribute(
									new CustomAttributeBuilder(
															typeof(CategoryAttribute).GetConstructor(new []{ typeof(String) }),
															new object[]{ option.Category }));
		}
		#endregion Methods
	}
}
