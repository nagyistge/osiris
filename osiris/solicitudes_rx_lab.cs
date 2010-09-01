// created on 16/08/2010 at 06:10 p
//////////////////////////////////////////////////////////////////////
// Sistema Hospitalario OSIRIS
// Monterrey - Mexico
//
// Autor    	: Ing. Daniel Olivares C. (Programacion) arcangeldoc@gmail.com
// 				  
// Licencia		: GLP
//////////////////////////////////////////////////////////
//
// proyect osiris is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// proyect osiris is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// 
//////////////////////////////////////////////////////////
// Programa		:
// Proposito	:  
// Objeto		: 
//////////////////////////////////////////////////////////
using System;
using Npgsql;
using System.Data;
using Gtk;
using Glade;

using GLib;
using System.Collections;

namespace osiris
{
	public class solicitudes_enfermeria
	{
		// Boton general para salir de las ventanas
		// Todas la ventanas en glade este boton debe estra declarado identico
		[Widget] Gtk.Button button_salir = null;
		
		[Widget] Gtk.Window solicitar_examen_labrx = null;
		[Widget] Gtk.RadioButton radiobutton_soli_interna = null;
		[Widget] Gtk.RadioButton radiobutton_soli_externa = null;		
		[Widget] Gtk.Entry entry_id_proveedor = null;
		[Widget] Gtk.Entry entry_nombre_proveedor = null;
		[Widget] Gtk.Button button_buscar_proveedor = null;
		[Widget] Gtk.Button button_busca_producto = null;
		
		/////// Ventana Busqueda de productos\\\\\\\\
		[Widget] Gtk.Window busca_producto = null;
		[Widget] Gtk.TreeView lista_de_producto;
		[Widget] Gtk.Entry entry_cantidad_aplicada;
		[Widget] Gtk.ComboBox combobox_tipo_admision;
		[Widget] Gtk.Entry entry_fecha_solicitud;
		[Widget] Gtk.Entry entry_hora_solicitud;
		[Widget] Gtk.Entry entry_folio_laboratorio;
		[Widget] Gtk.Label label_cantidad = null;
		// Para todas las busquedas este es el nombre asignado
		// se declara una vez
		[Widget] Gtk.Entry entry_expresion;
		[Widget] Gtk.Button button_selecciona;
		[Widget] Gtk.Button button_buscar_busqueda;
		
		string agrupacion_lab_rx;
		string descripinternamiento;
		int tipo_admisiones;
		string LoginEmpleado;
		string NomEmpleado;
		string AppEmpleado;
		string ApmEmpleado;
		
		string connectionString;
		string nombrebd;
		
		class_conexion conexion_a_DB = new class_conexion();
		class_buscador classfind_data = new class_buscador();
		class_public classpublic = new class_public();
		
		//Declaracion de ventana de error
		protected Gtk.Window MyWinError;
		protected Gtk.Window MyWin;
		
		public solicitudes_enfermeria(string LoginEmp_, string NomEmpleado_, string AppEmpleado_, string ApmEmpleado_, string nombrebd_,
		                              string departament_,int tipo_admisiones_,string agrupacion_lab_rx_,string descripinternamiento_)
		{
			LoginEmpleado = LoginEmp_;
			NomEmpleado = NomEmpleado_;
			AppEmpleado = AppEmpleado_;
			ApmEmpleado = ApmEmpleado_;
			connectionString = conexion_a_DB._url_servidor+conexion_a_DB._port_DB+conexion_a_DB._usuario_DB+conexion_a_DB._passwrd_user_DB;
			nombrebd = conexion_a_DB._nombrebd;
			agrupacion_lab_rx = agrupacion_lab_rx_;
			descripinternamiento = descripinternamiento_;
			tipo_admisiones = tipo_admisiones_;
			Glade.XML gxml = new Glade.XML (null, "hospitalizacion.glade", "solicitar_examen_labrx", null);
			gxml.Autoconnect (this); 
			
			// show the window
			solicitar_examen_labrx.Show();
			solicitar_examen_labrx.Title = departament_;
			entry_id_proveedor.Sensitive = false;
			entry_nombre_proveedor.Sensitive = false;
			button_buscar_proveedor.Sensitive = false;
			// Sale de la ventana
			button_salir.Clicked += new EventHandler(on_cierraventanas_clicked);
			//radiobutton_soli_interna
			radiobutton_soli_externa.Clicked += new EventHandler(on_radiobutton_soli_externa_clicked);
			button_buscar_proveedor.Clicked += new EventHandler(on_button_buscar_proveedor_clicked);
			button_busca_producto.Clicked += new EventHandler(on_button_busca_producto_clicked);
				
			entry_id_proveedor.Sensitive = false;
			entry_nombre_proveedor.Sensitive = false;
			button_buscar_proveedor.Sensitive = false;
		}
		
		void on_radiobutton_soli_externa_clicked(object sender, EventArgs args)
		{
			if(radiobutton_soli_externa.Active == true){
				entry_id_proveedor.Sensitive = true;
				entry_nombre_proveedor.Sensitive = true;
				button_buscar_proveedor.Sensitive = true;
			}else{
				entry_id_proveedor.Sensitive = false;
				entry_nombre_proveedor.Sensitive = false;
				button_buscar_proveedor.Sensitive = false;
			}			
		}
		
		void on_button_buscar_proveedor_clicked(object sender, EventArgs args){
			object[] parametros_objetos = {entry_id_proveedor,entry_nombre_proveedor};
			string[] parametros_sql = {"SELECT descripcion_proveedor,direccion_proveedor,rfc_proveedor,curp_proveedor, "+
								"colonia_proveedor,municipio_proveedor,estado_proveedor,telefono1_proveedor, "+ 
								"telefono2_proveedor,celular_proveedor,rfc_proveedor, proveedor_activo, "+
								"id_proveedor,contacto1_proveedor,mail_proveedor,pagina_web_proveedor,"+
								"osiris_erp_proveedores.id_forma_de_pago,descripcion_forma_de_pago,fax_proveedor "+
								"FROM osiris_erp_proveedores, osiris_erp_forma_de_pago "+
								"WHERE osiris_erp_proveedores.id_forma_de_pago = osiris_erp_forma_de_pago.id_forma_de_pago ",				
								"SELECT descripcion_proveedor,direccion_proveedor,rfc_proveedor,curp_proveedor, "+
								"colonia_proveedor,municipio_proveedor,estado_proveedor,telefono1_proveedor, "+ 
								"telefono2_proveedor,celular_proveedor,rfc_proveedor, proveedor_activo, "+
								"id_proveedor,contacto1_proveedor,mail_proveedor,pagina_web_proveedor, "+
								"osiris_erp_proveedores.id_forma_de_pago,descripcion_forma_de_pago,fax_proveedor "+
								"FROM osiris_erp_proveedores, osiris_erp_forma_de_pago "+
								"WHERE osiris_erp_proveedores.id_forma_de_pago = osiris_erp_forma_de_pago.id_forma_de_pago "+
								"AND descripcion_proveedor LIKE '%"};
			classfind_data.buscandor(parametros_objetos,parametros_sql,"find_proveedores_catalogo_producto"," ORDER BY descripcion_proveedor;","%' ",0);
		}
		
		void on_button_busca_producto_clicked (object sender, EventArgs args)
		{
			Glade.XML gxml = new Glade.XML (null, "laboratorio.glade", "busca_producto", null);
			gxml.Autoconnect (this);
			label_cantidad.Text = "Cantidad Solicitada";
			button_salir.Clicked += new EventHandler(on_cierraventanas_clicked);
			
			entry_fecha_solicitud.Text = DateTime.Now.ToString("yyyy-MM-dd");
			entry_hora_solicitud.Text = DateTime.Now.ToString("HH:mm:ss");
						
			// Validando que sean solo numeros
			entry_cantidad_aplicada.KeyPressEvent += onKeyPressEvent;
			entry_folio_laboratorio.KeyPressEvent += onKeyPressEvent;
			
			//SE LLENA EL COMBO BOX	
			combobox_tipo_admision.Clear();
			CellRendererText cell2 = new CellRendererText();
			combobox_tipo_admision.PackStart(cell2, true);
			combobox_tipo_admision.AddAttribute(cell2,"text",0);
	        
			ListStore store2 = new ListStore( typeof (string), typeof (int));
			combobox_tipo_admision.Model = store2;
			
			if(descripinternamiento == "*"){	        
		      	NpgsqlConnection conexion; 
				conexion = new NpgsqlConnection (connectionString+nombrebd);
	            // Verifica que la base de datos este conectada
	            try{
					conexion.Open ();
					NpgsqlCommand comando; 
					comando = conexion.CreateCommand ();
	               	comando.CommandText = "SELECT * FROM osiris_his_tipo_admisiones WHERE servicio_directo = 'false' "+
		           							"AND cuenta_mayor = 4000 "+
		               						" ORDER BY id_tipo_admisiones;";
					
					NpgsqlDataReader lector = comando.ExecuteReader ();
					store2.AppendValues ("", 0);
	               	while (lector.Read())
					{
						store2.AppendValues ((string) lector["descripcion_admisiones"], (int) lector["id_tipo_admisiones"]);
					}
				}catch (NpgsqlException ex){
					MessageDialog msgBoxError = new MessageDialog (MyWinError,DialogFlags.DestroyWithParent,
											MessageType.Error,ButtonsType.Close,"PostgresSQL error: {0}",ex.Message);
					msgBoxError.Run ();				msgBoxError.Destroy();
				}
				conexion.Close ();
			}else{
				store2.AppendValues (descripinternamiento,tipo_admisiones);
			}
			TreeIter iter2;
			if (store2.GetIterFirst(out iter2)) {
				//Console.WriteLine(iter2);
				combobox_tipo_admision.SetActiveIter (iter2);
			}
			combobox_tipo_admision.Changed += new EventHandler (onComboBoxChanged_tipo_admision);	    
		}
		
		void onComboBoxChanged_tipo_admision (object sender, EventArgs args)
		{
	    	ComboBox combobox_tipo_admision = sender as ComboBox;
			if (sender == null) { 		return; }
	  		TreeIter iter;
	  		if (combobox_tipo_admision.GetActiveIter (out iter)){
		    		//idlugarprocedencia = (int) combobox_tipo_admision.Model.GetValue(iter,1);
		    		//descriplugarprocedencia = (string) combobox_tipo_admision.Model.GetValue(iter,0);
		    		//Console.WriteLine(idlugarprocedencia+" "+descriplugarprocedencia);
	     	}
		}
		
		// Valida entradas que solo sean numericas, se utiliza eb ventana de
		// carga de producto
		[GLib.ConnectBefore ()]   	  // Esto es indispensable para que funcione    
		public void onKeyPressEvent(object o, Gtk.KeyPressEventArgs args)
		{
			string misDigitos = ".0123456789ﾰﾱﾲﾳﾴﾵﾶﾷﾸﾹﾮｔｒｓｑ（）";
			if (Array.IndexOf(misDigitos.ToCharArray(), Convert.ToChar(args.Event.Key)) == -1 && args.Event.Key != Gdk.Key.BackSpace){
				args.RetVal = true;
			}
		}
		
		void on_cierraventanas_clicked(object sender, EventArgs args)
		{
			Widget win = (Widget) sender;
			win.Toplevel.Destroy();
		}
		
	}	
	
	public class solicitudes_rx_lab
	{
		// Boton general para salir de las ventanas
		// Todas la ventanas en glade este boton debe estra declarado identico
		[Widget] Gtk.Button button_salir = null;
		
		[Widget] Gtk.Window solicitudes_examenes_labrx = null;
		[Widget] Gtk.Button button_consultar = null;
 		[Widget] Gtk.RadioButton radiobutton_estud_carg = null;
		[Widget] Gtk.RadioButton radiobutton_estud_solic = null;
		[Widget] Gtk.Notebook notebook1 = null;
		[Widget] Gtk.Entry entry_fecha_inicio = null;
		[Widget] Gtk.Entry entry_fecha_termino = null;
		[Widget] Gtk.CheckButton checkbutton_rango_fecha = null;
		
		// Tab number one application form request LAB RX
		[Widget] Gtk.TreeView treeview_lista_solicitados = null;
		[Widget] Gtk.Button button_cargar_examen = null;
		[Widget] Gtk.CheckButton checkbutton_px_solicitud = null;
		
		// Tab number two Charges to patients
		[Widget] Gtk.TreeView treeview_lista_cargosvalid = null;
		[Widget] Gtk.Button button_validar_examen = null;
		
		string LoginEmpleado;
		string NomEmpleado;
		string AppEmpleado;
		string ApmEmpleado;
		string nombrebd;
		class_conexion conexion_a_DB = new class_conexion();
		
		public solicitudes_rx_lab(string LoginEmp_, string NomEmpleado_, string AppEmpleado_, string ApmEmpleado_, string nombrebd_,string departament_,int tipo_admisiones_)
		{			
			Glade.XML gxml = new Glade.XML (null, "imagenologia.glade", "solicitudes_examenes_labrx", null);
			gxml.Autoconnect (this);	        
			// Muestra ventana de Glade
			solicitudes_examenes_labrx.Show();
			solicitudes_examenes_labrx.Title = departament_;
			
			LoginEmpleado = LoginEmp_;
			NomEmpleado = NomEmpleado_;
			AppEmpleado = AppEmpleado_;
			ApmEmpleado = ApmEmpleado_;
			nombrebd = conexion_a_DB._nombrebd;
			
			// Sale de la ventana
			button_salir.Clicked += new EventHandler(on_cierraventanas_clicked);
			radiobutton_estud_carg.Clicked += new EventHandler(on_changetab_clicked);
			radiobutton_estud_solic.Clicked += new EventHandler(on_changetab_clicked);
			button_consultar.Clicked += new EventHandler(on_button_consultar_clicked);
			button_cargar_examen.Clicked += new EventHandler(on_button_cargar_examen_clicked);
			button_consultar.Clicked += new EventHandler(on_button_consultar_clicked);
			entry_fecha_inicio.Text = DateTime.Now.ToString("yyyy-MM-dd");
			entry_fecha_termino.Text = DateTime.Now.ToString("yyyy-MM-dd");
			checkbutton_rango_fecha.Active = true;
			
			create_treeview_solicitudes((bool) checkbutton_px_solicitud.Active);
			create_treeview_cargados();
		}
		
		void on_button_consultar_clicked(object sender, EventArgs args)
		{
			if(radiobutton_estud_carg.Active == true){
				notebook1.CurrentPage = 1;
			}
			if(radiobutton_estud_solic.Active == true){
				notebook1.CurrentPage = 0;
				create_treeview_solicitudes((bool) checkbutton_px_solicitud.Active);
			}
		}
		
		void on_changetab_clicked(object sender, EventArgs args)
		{
			//Console.WriteLine(radiobutton_seltab.Active.ToString());
			Gtk.RadioButton radiobutton_seltab = (Gtk.RadioButton) sender;
			
			if(radiobutton_seltab.Name.ToString() ==  "radiobutton_estud_carg"){
				notebook1.CurrentPage = 1;
			}
			if(radiobutton_seltab.Name.ToString() ==  "radiobutton_estud_solic"){
				notebook1.CurrentPage = 0;
			}
		}
		
		void on_button_cargar_examen_clicked(object sender, EventArgs args)
		{
			new osiris.DemoTreeStore();
		}
			
		void create_treeview_solicitudes(bool tipo_treeview)
		{
			Gtk.TreeStore treeViewEnginesolicitados;
			
			// create treeview List the request
			if(tipo_treeview == false){
				// Erase all columns
				foreach (TreeViewColumn tvc in this.treeview_lista_solicitados.Columns)
				this.treeview_lista_solicitados.RemoveColumn(tvc);
				treeViewEnginesolicitados = new TreeStore(typeof(string),typeof(string),typeof(string),typeof(string),typeof(string),
													typeof(string),typeof(string),typeof(string),typeof(string));
				treeview_lista_solicitados.Model = treeViewEnginesolicitados;
				treeview_lista_solicitados.RulesHint = true;
				
				Gtk.TreeViewColumn col_request0 = new TreeViewColumn();	Gtk.CellRendererToggle cellrt0 = new CellRendererToggle();		
				col_request0.Title = "Seleccion";
				col_request0.PackStart(cellrt0, true);
				col_request0.AddAttribute (cellrt0, "active", 0);
				//col_request0.Resizable = true;
				//col_request0.SortColumnId = (int) coldatos_request.col_request0;
				
				Gtk.TreeViewColumn col_request1 = new TreeViewColumn();		Gtk.CellRendererText cellrt1 = new Gtk.CellRendererText();		
				col_request1.Title = "Nº Solicitud";
				col_request1.PackStart(cellrt1, true);
				col_request1.AddAttribute (cellrt1, "text", 1);
				//col_request1.Resizable = true;
				//col_request1.SortColumnId = (int) coldatos_request.col_request1;
							
				treeview_lista_solicitados.AppendColumn(col_request0);
				treeview_lista_solicitados.AppendColumn(col_request1);
				llenado_treeview_solicitudes((bool) checkbutton_px_solicitud.Active,treeViewEnginesolicitados);
				
			}else{
				// create treeview for patients
				// Erase all columns
				foreach (TreeViewColumn tvc in this.treeview_lista_solicitados.Columns)
				this.treeview_lista_solicitados.RemoveColumn(tvc);
				//treeview_lista_solicitados.Remove();
				
				treeViewEnginesolicitados = new TreeStore(typeof(string),typeof(bool),typeof(string),typeof(string),typeof(string),
													typeof(string),typeof(string),typeof(string),typeof(string));
				treeview_lista_solicitados.Model = treeViewEnginesolicitados;
				treeview_lista_solicitados.RulesHint = true;
				treeview_lista_solicitados.Selection.Mode = SelectionMode.Multiple;
											
				Gtk.TreeViewColumn col_request0 = new TreeViewColumn();		Gtk.CellRendererText cellrt0 = new Gtk.CellRendererText();		
				col_request0.Title = "Paciente/Estudio";
				col_request0.PackStart(cellrt0, true);
				col_request0.AddAttribute (cellrt0, "text", 0);
				col_request0.SortOrder = SortType.Descending;
				cellrt0.Xalign = 0.0f;
				
				Gtk.TreeViewColumn col_request1 = new TreeViewColumn();	Gtk.CellRendererToggle cellrt1 = new CellRendererToggle();		
				col_request1.Title = "Seleccion";
				col_request1.PackStart(cellrt1, true);
				col_request1.AddAttribute (cellrt1, "active", 1);
				//col_request0.Resizable = true;
				//col_request0.SortColumnId = (int) coldatos_request.col_request0;
				
				Gtk.TreeViewColumn col_request2 = new TreeViewColumn();		Gtk.CellRendererText cellrt2 = new Gtk.CellRendererText();		
				col_request2.Title = "Nº Solicitud";
				col_request2.PackStart(cellrt2, true);
				col_request2.AddAttribute (cellrt2, "text", 2);
				//col_request1.Resizable = true;
				//col_request1.SortColumnId = (int) coldatos_request.col_request1;
				
				Gtk.TreeViewColumn col_request3 = new TreeViewColumn();		Gtk.CellRendererText cellrt3 = new Gtk.CellRendererText();
				col_request3.Title = "Cant.Solicitado";
				col_request3.PackStart(cellrt3, true);
				col_request3.AddAttribute (cellrt3, "text", 3);
				//col_request1.Resizable = true;
				//col_request1.SortColumnId = (int) coldatos_request.col_request1;
				
				Gtk.TreeViewColumn col_request4 = new TreeViewColumn();		Gtk.CellRendererText cellrt4 = new Gtk.CellRendererText();
				col_request4.Title = "Codigo";
				col_request4.PackStart(cellrt4, true);
				col_request4.AddAttribute (cellrt4, "text", 4);
				//col_request1.Resizable = true;
				//col_request1.SortColumnId = (int) coldatos_request.col_request1;
							
				treeview_lista_solicitados.AppendColumn(col_request0);
				treeview_lista_solicitados.AppendColumn(col_request1);
				treeview_lista_solicitados.AppendColumn(col_request2);
				treeview_lista_solicitados.AppendColumn(col_request3);
				treeview_lista_solicitados.AppendColumn(col_request4);
				llenado_treeview_solicitudes((bool) checkbutton_px_solicitud.Active,treeViewEnginesolicitados);
			}
		}
		
		void llenado_treeview_solicitudes(bool tipo_treeview, object obj)
		{
			Gtk.TreeStore treeViewEnginesolicitados = (Gtk.TreeStore) obj;
			Gtk.TreeIter iter;
			// llenado de lista de solicitudes
			if(tipo_treeview == false){
				
			}
			// llenado por paciente
			if(tipo_treeview == true){
				iter = treeViewEnginesolicitados.AppendValues("paciente_01");
				treeViewEnginesolicitados.AppendValues(iter,"RX. MUÑECA  AP Y OBLICUA  ( 2 POSICIONES ) BILATERAL COMPARATIVA",false,"123","1");
				treeViewEnginesolicitados.AppendValues(iter,"RX. MUSLO O FEMUR  ( 1 POSICION )",true,"124","1");
				treeViewEnginesolicitados.AppendValues(iter,"RX. ESTERNON ( 1 POSICI )",false,"124","1");
				iter = treeViewEnginesolicitados.AppendValues("paciente_02");
				
				treeview_lista_solicitados.ExpandAll();
			}
			
		}
		
		void create_treeview_cargados()
		{
			//treeViewEnginecargosvalid = new TreeStore(typeof(bool),typeof(string),typeof(string),typeof(string),typeof(string),typeof(string),
			//									typeof(string),typeof(string),typeof(string),typeof(string));
			//treeview_lista_cargosvalid.Model = treeViewEnginecargosvalid;
			//treeview_lista_cargosvalid.RulesHint = true;
		}
		
		
		// cierra ventanas emergentes
		void on_cierraventanas_clicked (object sender, EventArgs args)
		{
			Widget win = (Widget) sender;
			win.Toplevel.Destroy();
		}
	}
	
		public class DemoTreeStore : Gtk.Window
	{
		private TreeStore store;

		public DemoTreeStore () : base ("Card planning sheet")
		{
			VBox vbox = new VBox (false, 8);
			vbox.BorderWidth = 8;
			Add (vbox);

			vbox.PackStart (new Label ("Jonathan's Holiday Card Planning Sheet"),
					false, false, 0);

			ScrolledWindow sw = new ScrolledWindow ();
			sw.ShadowType = ShadowType.EtchedIn;
			sw.SetPolicy (PolicyType.Automatic, PolicyType.Automatic);
			vbox.PackStart (sw, true, true, 0);

			// create model
			CreateModel ();

			// create tree view
			TreeView treeView = new TreeView (store);
			treeView.RulesHint = true;
			treeView.Selection.Mode = SelectionMode.Multiple;
			AddColumns (treeView);

			sw.Add (treeView);

			// expand all rows after the treeview widget has been realized
			treeView.Realized += new EventHandler (ExpandRows);

			SetDefaultSize (650, 400);
			ShowAll ();
		}

		private void ExpandRows (object obj, EventArgs args)
		{
			TreeView treeView = obj as TreeView;

			treeView.ExpandAll ();
		}

		ArrayList columns = new ArrayList ();

		private void ItemToggled (object sender, ToggledArgs args)
		{
			int column = columns.IndexOf (sender);

 			Gtk.TreeIter iter;
 			if (store.GetIterFromString (out iter, args.Path)) {
 				bool val = (bool) store.GetValue (iter, column);
 				store.SetValue (iter, column, !val);
 			}
		}

		private void AddColumns (TreeView treeView)
		{
			CellRendererText text;
			CellRendererToggle toggle;

			// column for holiday names
			text = new CellRendererText ();
			text.Xalign = 0.0f;
			columns.Add (text);
			TreeViewColumn column = new TreeViewColumn ("Holiday", text,
								    "text", Column.HolidayName);
			treeView.InsertColumn (column, (int) Column.HolidayName);

			// alex column
			toggle = new CellRendererToggle ();
			toggle.Xalign = 0.0f;
			columns.Add (toggle);
			toggle.Toggled += new ToggledHandler (ItemToggled);
			column = new TreeViewColumn ("Alex", toggle,
						     "active", (int) Column.Alex,
						     "visible", (int) Column.Visible,
						     "activatable", (int) Column.World);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 50;
			column.Clickable = true;
			treeView.InsertColumn (column, (int) Column.Alex);

			// havoc column
			toggle = new CellRendererToggle ();
			toggle.Xalign = 0.0f;
			columns.Add (toggle);
			toggle.Toggled += new ToggledHandler (ItemToggled);
			column = new TreeViewColumn ("Havoc", toggle,
						     "active", (int) Column.Havoc,
						     "visible", (int) Column.Visible);
			treeView.InsertColumn (column, (int) Column.Havoc);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 50;
			column.Clickable = true;

			// tim column
			toggle = new CellRendererToggle ();
			toggle.Xalign = 0.0f;
			columns.Add (toggle);
			toggle.Toggled += new ToggledHandler (ItemToggled);
			column = new TreeViewColumn ("Tim", toggle,
						     "active", (int) Column.Tim,
						     "visible", (int) Column.Visible,
						     "activatable", (int) Column.World);
			treeView.InsertColumn (column, (int) Column.Tim);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 50;
			column.Clickable = true;

			// owen column
			toggle = new CellRendererToggle ();
			toggle.Xalign = 0.0f;
			columns.Add (toggle);
			toggle.Toggled += new ToggledHandler (ItemToggled);
			column = new TreeViewColumn ("Owen", toggle,
						     "active", (int) Column.Owen,
						     "visible", (int) Column.Visible);
			treeView.InsertColumn (column, (int) Column.Owen);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 50;
			column.Clickable = true;

			// dave column
			toggle = new CellRendererToggle ();
			toggle.Xalign = 0.0f;
			columns.Add (toggle);
			toggle.Toggled += new ToggledHandler (ItemToggled);
			column = new TreeViewColumn ("Dave", toggle,
						     "active", (int) Column.Dave,
						     "visible", (int) Column.Visible);
			treeView.InsertColumn (column, (int) Column.Dave);
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 50;
			column.Clickable = true;
		}

		protected override bool OnDeleteEvent (Gdk.Event evt)
		{
			Destroy ();
			return true;
		}

		private void CreateModel ()
		{
			// create tree store
			store = new TreeStore (typeof (string),
					       typeof (bool),
					       typeof (bool),
					       typeof (bool),
					       typeof (bool),
					       typeof (bool),
					       typeof (bool),
					       typeof (bool));

			// add data to the tree store
			foreach (MyTreeItem month in toplevel) {
				TreeIter iter = store.AppendValues (month.Label,
								    false,
								    false,
								    false,
								    false,
								    false,
								    false,
								    false);

				foreach (MyTreeItem holiday in month.Children) {
					store.AppendValues (iter,
							    holiday.Label,
							    holiday.Alex,
							    holiday.Havoc,
							    holiday.Tim,
							    holiday.Owen,
							    holiday.Dave,
							    true,
							    holiday.WorldHoliday);
				}
			}
		}

		// tree data
		private static MyTreeItem[] january =
		{
			new MyTreeItem ("New Years Day", true, true, true, true, false, true, null ),
			new MyTreeItem ("Presidential Inauguration", false, true, false, true, false, false, null ),
			new MyTreeItem ("Martin Luther King Jr. day", false, true, false, true, false, false, null )
		};

		private static MyTreeItem[] february =
		{
			new MyTreeItem ( "Presidents' Day", false, true, false, true, false, false, null ),
			new MyTreeItem ( "Groundhog Day", false, false, false, false, false, false, null ),
			new MyTreeItem ( "Valentine's Day", false, false, false, false, true, true, null )
		};

		private static MyTreeItem[] march =
		{
			new MyTreeItem ( "National Tree Planting Day", false, false, false, false, false, false, null ),
			new MyTreeItem ( "St Patrick's Day", false, false, false, false, false, true, null )
		};

		private static MyTreeItem[] april =
		{
			new MyTreeItem ( "April Fools' Day", false, false, false, false, false, true, null ),
			new MyTreeItem ( "Army Day", false, false, false, false, false, false, null ),
			new MyTreeItem ( "Earth Day", false, false, false, false, false, true, null ),
			new MyTreeItem ( "Administrative Professionals' Day", false, false, false, false, false, false, null )
		};

		private static MyTreeItem[] may =
		{
			new MyTreeItem ( "Nurses' Day", false, false, false, false, false, false, null ),
			new MyTreeItem ( "National Day of Prayer", false, false, false, false, false, false, null ),
			new MyTreeItem ( "Mothers' Day", false, false, false, false, false, true, null ),
			new MyTreeItem ( "Armed Forces Day", false, false, false, false, false, false, null ),
			new MyTreeItem ( "Memorial Day", true, true, true, true, false, true, null )
		};

		private static MyTreeItem[] june =
		{
			new MyTreeItem ( "June Fathers' Day", false, false, false, false, false, true, null ),
			new MyTreeItem ( "Juneteenth (Liberation of Slaves)", false, false, false, false, false, false, null ),
			new MyTreeItem ( "Flag Day", false, true, false, true, false, false, null )
		};

		private static MyTreeItem[] july =
		{
			new MyTreeItem ( "Parents' Day", false, false, false, false, false, true, null ),
			new MyTreeItem ( "Independence Day", false, true, false, true, false, false, null )
		};

		private static MyTreeItem[] august =
		{
			new MyTreeItem ( "Air Force Day", false, false, false, false, false, false, null ),
			new MyTreeItem ( "Coast Guard Day", false, false, false, false, false, false, null ),
			new MyTreeItem ( "Friendship Day", false, false, false, false, false, false, null )
		};

		private static MyTreeItem[] september =
		{
			new MyTreeItem ( "Grandparents' Day", false, false, false, false, false, true, null ),
			new MyTreeItem ( "Citizenship Day or Constitution Day", false, false, false, false, false, false, null ),
			new MyTreeItem ( "Labor Day", true, true, true, true, false, true, null )
		};

		private static MyTreeItem[] october =
		{
			new MyTreeItem ( "National Children's Day", false, false, false, false, false, false, null ),
			new MyTreeItem ( "Bosses' Day", false, false, false, false, false, false, null ),
			new MyTreeItem ( "Sweetest Day", false, false, false, false, false, false, null ),
			new MyTreeItem ( "Mother-in-Law's Day", false, false, false, false, false, false, null ),
			new MyTreeItem ( "Navy Day", false, false, false, false, false, false, null ),
			new MyTreeItem ( "Columbus Day", false, true, false, true, false, false, null ),
			new MyTreeItem ( "Halloween", false, false, false, false, false, true, null )
		};

		private static MyTreeItem[] november =
		{
			new MyTreeItem ( "Marine Corps Day", false, false, false, false, false, false, null ),
			new MyTreeItem ( "Veterans' Day", true, true, true, true, false, true, null ),
			new MyTreeItem ( "Thanksgiving", false, true, false, true, false, false, null )
		};

		private static MyTreeItem[] december =
		{
			new MyTreeItem ( "Pearl Harbor Remembrance Day", false, false, false, false, false, false, null ),
			new MyTreeItem ( "Christmas", true, true, true, true, false, true, null ),
			new MyTreeItem ( "Kwanzaa", false, false, false, false, false, false, null )
		};


		private static MyTreeItem[] toplevel =
		{
			new MyTreeItem ("January", false, false, false, false, false, false, january),
			new MyTreeItem ("February", false, false, false, false, false, false, february),
			new MyTreeItem ("March", false, false, false, false, false, false, march),
			new MyTreeItem ("April", false, false, false, false, false, false, april),
			new MyTreeItem ("May", false, false, false, false, false, false, may),
			new MyTreeItem ("June", false, false, false, false, false, false, june),
			new MyTreeItem ("July", false, false, false, false, false, false, july),
			new MyTreeItem ("August", false, false, false, false, false, false, august),
			new MyTreeItem ("September", false, false, false, false, false, false, september),
			new MyTreeItem ("October", false, false, false, false, false, false, october),
			new MyTreeItem ("November", false, false, false, false, false, false, november),
			new MyTreeItem ("December", false, false, false, false, false, false, december)
		};

		// TreeItem structure
		public class MyTreeItem
		{
			public string Label;
			public bool Alex, Havoc, Tim, Owen, Dave;
			public bool WorldHoliday; // shared by the European hackers
			public MyTreeItem[] Children;

			public MyTreeItem (string label, bool alex, bool havoc, bool tim,
					   bool owen, bool dave, bool worldHoliday,
					   MyTreeItem[] children)
			{
				Label = label;
				Alex = alex;
				Havoc = havoc;
				Tim = tim;
				Owen = owen;
				Dave = dave;
				WorldHoliday =  worldHoliday;
				Children = children;
			}
		}

		// columns
		public enum Column
		{
			HolidayName,
			Alex,
			Havoc,
			Tim,
			Owen,
			Dave,

			Visible,
			World,
		}
	}
}
