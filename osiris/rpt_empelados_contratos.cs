////////////////////////////////////////////////////////////////////////////////////////////////////
// created on 1/04/2008                                                                           
// Sistema Hospitalario OSIRIS                                                                                          
// Monterrey - Mexico                                                                                                
//                                                                                                                            
// Autor    	: Erick Eduardo Gonzalez Reyes (Programation & Glade's window)            
//                                                                                                       
// Licencia		: GLP                                                                                                  
////////////////////////////////////////////////////////////////////////////////////////////////////
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
/////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using Npgsql;
using System.Data;
using Gtk;
using Glade;

namespace osiris
{	
	public class reportes_empleados
	{	
		[Widget] Gtk.Window reportes_de_empleados;
		[Widget] Gtk.Button button_salir;
		[Widget] Gtk.Button button_contrato;
		[Widget] Gtk.Button button_bajas;
		[Widget] Gtk.Button button_Imprimir;
		[Widget] Gtk.TreeView treeview_reportelista;
		[Widget] Gtk.Entry entry_dia_inicio;
		[Widget] Gtk.Entry entry_mes_inicio;
		[Widget] Gtk.Entry entry_anno_inicio;
		[Widget] Gtk.Entry entry_dia_fin;
		[Widget] Gtk.Entry entry_mes_fin;
		[Widget] Gtk.Entry entry_anno_fin;
		[Widget] Gtk.ComboBox combo_tipocontrato;
		[Widget] Gtk.CheckButton check_todos;
		[Widget] Gtk.Label label_cont;	
	
		string connectionString;
		string nombrebd;
		string LoginEmpleado;
    	string NomEmpleado;
    	string AppEmpleado;
    	string ApmEmpleado;
        string fechamax;
        string fechamin;
        string fechamaxsql;
        string fechaminsql;
        string id_empleado;
        string var_fecha;
        string var_tipo;
        string var_id_empleado;
        string var_sueldo;
        string var_depto;
        string var_puesto;
        string var_tipofuncion;
        string var_horas;
        string var_tiempocomida;
        string var_tipopago;
        string var_jornada;
        string var_annos;
        string id_sel;
        string nombre_completo;
        string fechacontrato;
        int conteo_lineas = 0;
        bool leer = false;
        bool fecha_valida = false;
        string tiempo_de_contrato_short="";
        
        string fechadebaja = "";
        
        string contrato;
        string fechacontr;
		string tpcontrato;		
		string tipfun;		
		string depart;	
		string puest;	
		string jorn;	
		string suel;	
		string tippag;	
		string tpocomida;
		string annosemp;
        
    	private TreeStore treeViewEngineBusca;
    	
    	protected Gtk.Window MyWinError;
		protected Gtk.Window MyWin;
		
		class_conexion conexion_a_DB = new class_conexion();
		
		public reportes_empleados(string LoginEmp_, string NomEmpleado_, string AppEmpleado_, string ApmEmpleado_, string nombrebd_,string tipo_reporte_)
		{
			LoginEmpleado = LoginEmp_;
    		NomEmpleado = NomEmpleado_;
    		AppEmpleado = AppEmpleado_;
    		ApmEmpleado = ApmEmpleado_;
			
    		connectionString = conexion_a_DB._url_servidor+conexion_a_DB._port_DB+conexion_a_DB._usuario_DB+conexion_a_DB._passwrd_user_DB;
			nombrebd = conexion_a_DB._nombrebd;
    		    		
			Glade.XML gxml = new Glade.XML (null, "recursos_humanos.glade", "reportes_de_empleados",null);
			gxml.Autoconnect (this);
	        reportes_de_empleados.Show();
			
			button_contrato.Clicked += new EventHandler(on_contrato_clicked);
			button_bajas.Clicked += new EventHandler(on_bajas_clicked);
			button_salir.Clicked += new EventHandler(on_cierraventanas_clicked);
			button_Imprimir.Clicked += new EventHandler(on_imprimir_clicked);
			check_todos.Clicked  += new EventHandler(on_check_todos_clicked);
			
			llenacombo_contrato();
			
			if (tipo_reporte_ == "Contrato"){
				crea_treeview_busqueda_empleado();
				this.button_bajas.Visible = false;
				this.button_contrato.Visible = false;
			}
		    
		    if (tipo_reporte_ == "Bajas"){
		    	crea_treeview_busqueda_bajas();
			
				this.button_bajas.Visible = true;
				this.combo_tipocontrato.Visible = false;
				this.button_contrato.Visible = false;
				this.label_cont.Visible = false;
				this.check_todos.Visible = false;
				this.button_Imprimir.Visible = false;
			}
		    
		    this.entry_dia_inicio.Text = DateTime.Now.ToString("dd");
		    this.entry_mes_inicio.Text = DateTime.Now.ToString("MM");
		    this.entry_anno_inicio.Text = DateTime.Now.ToString("yyyy");
		     
		    this.entry_dia_fin.Text = DateTime.Now.ToString("dd");
		    this.entry_mes_fin.Text = DateTime.Now.ToString("MM");
		    this.entry_anno_fin.Text = DateTime.Now.ToString("yyyy");		     
		}
		
		void on_check_todos_clicked(object sender, EventArgs args)
		{ 
			if (this.check_todos.Active == true){
				this.combo_tipocontrato.Sensitive = false;
				llena_lista_contrato();
		    }else{
				this.combo_tipocontrato.Sensitive = true;
				llena_lista_contrato();
		    }
		}
		
		void llenacombo_contrato()
		{
			combo_tipocontrato.Clear();
			CellRendererText cell = new CellRendererText();
			combo_tipocontrato.PackStart(cell, true);
			combo_tipocontrato.AddAttribute(cell,"text",0);
			
			ListStore store = new ListStore( typeof (string));
			combo_tipocontrato.Model = store;
		    
		    store.AppendValues ((string) "");
            store.AppendValues ((string) "DETERMINADO (1 MES)");
	        store.AppendValues ((string) "DETERMINADO (2 MESES)");
	        store.AppendValues ((string) "DETERMINADO (3 MESES)");
	        store.AppendValues ((string) "INDETERMINADO");
	        store.AppendValues ((string) "HONORARIOS (ASIMILABLES)");
	        store.AppendValues ((string) "PRACTICAS");
	        
			TreeIter iter;
			if (store.GetIterFirst(out iter)){
				combo_tipocontrato.SetActiveIter (iter);
			 }
			combo_tipocontrato.Changed += new EventHandler (onComboBoxChanged_tipocontrato);
		   
		}
		
		void onComboBoxChanged_tipocontrato (object sender, EventArgs args)
		{
			ComboBox combo_tipocontrato = sender as ComboBox;
		    if (sender == null){
		    	return;
		    }
			TreeIter iter;
			if (combo_tipocontrato.GetActiveIter (out iter)) {
				tiempo_de_contrato_short = ((string) this.combo_tipocontrato.Model.GetValue(iter,0));
				llena_lista_contrato();
			}
		}
		
		
		void on_bajas_clicked (object sender, EventArgs args)
		{
			llena_lista_bajas();
		}
		
		void on_contrato_clicked (object sender, EventArgs args)
		{
		
			fechamin =  Convert.ToDateTime(this.entry_anno_inicio.Text+"/"+this.entry_mes_inicio.Text+"/"+this.entry_dia_inicio.Text).ToShortDateString();
			fechamax =  Convert.ToDateTime(this.entry_anno_fin.Text+"/"+this.entry_mes_fin.Text+"/"+this.entry_dia_fin.Text).ToShortDateString();
			
			Console.WriteLine(Convert.ToDateTime(fechamin).ToLongDateString());
			Console.WriteLine(Convert.ToDateTime(fechamax).ToLongDateString());
			
			if (Convert.ToDateTime(fechamin) > Convert.ToDateTime(fechamax)){
					MessageDialog msgBoxError = new MessageDialog (MyWinError,DialogFlags.DestroyWithParent,
					MessageType.Error,
					ButtonsType.Close,"La fecha de inicio debe de ser menor a la fecha final");
				msgBoxError.Run ();
				msgBoxError.Destroy();
			}else{
				llena_lista_contrato();
			}
		}
				
		void crea_treeview_busqueda_bajas()
		{
			treeViewEngineBusca = new TreeStore(typeof(string),typeof(string),typeof(string),typeof(string));
			treeview_reportelista.Model = treeViewEngineBusca;
			
			treeview_reportelista.RulesHint = true;			
			
			TreeViewColumn col_idEmpleado = new TreeViewColumn();
			CellRendererText cellr0 = new CellRendererText();
			col_idEmpleado.Title = "Empleado"; // titulo de la cabecera de la columna, si está visible
			col_idEmpleado.PackStart(cellr0, true);
			col_idEmpleado.AddAttribute (cellr0, "text", 0);    // la siguiente columna será 1 en vez de 1
			col_idEmpleado.SortColumnId = (int) Column2.col_idEmpleado;
			
			TreeViewColumn col_Empleado = new TreeViewColumn();
			CellRendererText cellr1 = new CellRendererText();
			col_Empleado.Title = "ID"; // titulo de la cabecera de la columna, si está visible
			col_Empleado.PackStart(cellr1, true);
			col_Empleado.AddAttribute (cellr1, "text", 1);    // la siguiente columna será 1 en vez de 1
			col_Empleado.SortColumnId = (int) Column2.col_Empleado;
			   // Permite edita este campo
            
			TreeViewColumn col_fecha = new TreeViewColumn();
			CellRendererText cellrt2 = new CellRendererText();
			col_fecha.Title = "Fecha de Baja";
			col_fecha.PackStart(cellrt2, true);
			col_fecha.AddAttribute (cellrt2, "text", 2); // la siguiente columna será 1 en vez de 2
			col_fecha.SortColumnId = (int) Column2.col_fecha;
            
			TreeViewColumn col_causa = new TreeViewColumn();
			CellRendererText cellrt3 = new CellRendererText();
			col_causa.Title = "Causa Baja";
			col_causa.PackStart(cellrt3, true);
			col_causa.AddAttribute (cellrt3, "text", 3); // la siguiente columna será 2 en vez de 3
			col_causa.SortColumnId = (int) Column2.col_causa;
			
			treeview_reportelista.AppendColumn(col_idEmpleado);
			treeview_reportelista.AppendColumn(col_Empleado);
			treeview_reportelista.AppendColumn(col_fecha);
			treeview_reportelista.AppendColumn(col_causa);
		 }
				
		void crea_treeview_busqueda_empleado()
		{			    
			treeViewEngineBusca = new TreeStore(typeof(string),typeof(string),typeof(string),typeof(string),typeof(string),typeof(string),typeof(string),typeof(string),typeof(string),typeof(string),typeof(string),typeof(string));
			treeview_reportelista.Model = treeViewEngineBusca;
			
			treeview_reportelista.RulesHint = true;
			
			treeview_reportelista.RowActivated += on_selecciona_empleado_clicked;  // Doble click selecciono paciente
			
			TreeViewColumn nom_Empleado = new TreeViewColumn();
			CellRendererText cellr0 = new CellRendererText();
			nom_Empleado.Title = "Empleado"; // titulo de la cabecera de la columna, si está visible
			nom_Empleado.PackStart(cellr0, true);
			nom_Empleado.AddAttribute (cellr0, "text", 0);    // la siguiente columna será 1 en vez de 1
			nom_Empleado.SortColumnId = (int) Column.nom_Empleado;
			
			TreeViewColumn col_Empleado = new TreeViewColumn();
			CellRendererText cellr1 = new CellRendererText();
			col_Empleado.Title = "ID"; // titulo de la cabecera de la columna, si está visible
			col_Empleado.PackStart(cellr1, true);
			col_Empleado.AddAttribute (cellr1, "text", 1);    // la siguiente columna será 1 en vez de 1
			col_Empleado.SortColumnId = (int) Column.col_Empleado;
			   // Permite edita este campo
            
			TreeViewColumn col_fecha = new TreeViewColumn();
			CellRendererText cellrt2 = new CellRendererText();
			col_fecha.Title = "Fecha de contrato";
			col_fecha.PackStart(cellrt2, true);
			col_fecha.AddAttribute (cellrt2, "text", 2); // la siguiente columna será 1 en vez de 2
			col_fecha.SortColumnId = (int) Column.col_fecha;
            
			TreeViewColumn col_tipo = new TreeViewColumn();
			CellRendererText cellrt3 = new CellRendererText();
			col_tipo.Title = "Tipo de contrato";
			col_tipo.PackStart(cellrt3, true);
			col_tipo.AddAttribute (cellrt3, "text", 3); // la siguiente columna será 2 en vez de 3
			col_tipo.SortColumnId = (int) Column.col_tipo;
            
            TreeViewColumn col_sueldo = new TreeViewColumn();
			CellRendererText cellrt4 = new CellRendererText();
			col_sueldo.Title = "Sueldo";
			col_sueldo.PackStart(cellrt4, true);
			col_sueldo.AddAttribute (cellrt4, "text",4); // la siguiente columna será 2 en vez de 3
			col_sueldo.SortColumnId = (int) Column.col_sueldo;
			
			TreeViewColumn col_depto = new TreeViewColumn();
			CellRendererText cellrt5 = new CellRendererText();
			col_depto.Title = "Departamento";
			col_depto.PackStart(cellrt5, true);
			col_depto.AddAttribute (cellrt5, "text", 5); // la siguiente columna será 2 en vez de 3
			col_depto.SortColumnId = (int) Column.col_depto;
			
            TreeViewColumn col_puesto = new TreeViewColumn();
			CellRendererText cellrt6 = new CellRendererText();
			col_puesto.Title = "Puesto";
			col_puesto.PackStart(cellrt6, true);
			col_puesto.AddAttribute (cellrt6, "text", 6); // la siguiente columna será 2 en vez de 3
			col_puesto.SortColumnId = (int) Column.col_puesto;
            
            TreeViewColumn col_jornada = new TreeViewColumn();
			CellRendererText cellrt7 = new CellRendererText();
			col_jornada.Title = "Jornada";
			col_jornada.PackStart(cellrt7, true);
			col_jornada.AddAttribute (cellrt7, "text", 7); // la siguiente columna será 2 en vez de 3
			col_jornada.SortColumnId = (int) Column.col_jornada;
            
            TreeViewColumn col_tipofuncion = new TreeViewColumn();
			CellRendererText cellrt8 = new CellRendererText();
			col_tipofuncion.Title = "Tipo de Función";
			col_tipofuncion.PackStart(cellrt8, true);
			col_tipofuncion.AddAttribute (cellrt8, "text", 8); // la siguiente columna será 2 en vez de 3
			col_tipofuncion.SortColumnId = (int) Column.col_tipofuncion;
            
            TreeViewColumn col_tipopago = new TreeViewColumn();
			CellRendererText cellrt9 = new CellRendererText();
			col_tipopago.Title = "Tipo de Pago";
			col_tipopago.PackStart(cellrt9, true);
			col_tipopago.AddAttribute (cellrt9, "text", 9); // la siguiente columna será 2 en vez de 3
			col_tipopago.SortColumnId = (int) Column.col_tipopago;
			
			TreeViewColumn col_tipocomida = new TreeViewColumn();
			CellRendererText cellrt10 = new CellRendererText();
			col_tipocomida.Title = "tiempo comida";
			col_tipocomida.PackStart(cellrt10, true);
			col_tipocomida.AddAttribute (cellrt10, "text", 10); // la siguiente columna será 2 en vez de 3
			col_tipocomida.SortColumnId = (int) Column.col_tipocomida;
            col_tipocomida.Visible = false;
            
            TreeViewColumn col_edad = new TreeViewColumn();
			CellRendererText cellrt11 = new CellRendererText();
			col_edad.Title = "Edad";
			col_edad.PackStart(cellrt11, true);
			col_edad.AddAttribute (cellrt11, "text", 11); // la siguiente columna será 2 en vez de 3
			col_edad.SortColumnId = (int) Column.col_edad;
            col_edad.Visible = false;
            
            treeview_reportelista.AppendColumn(nom_Empleado);
			treeview_reportelista.AppendColumn(col_Empleado);
			treeview_reportelista.AppendColumn(col_fecha);
			treeview_reportelista.AppendColumn(col_tipo);
			treeview_reportelista.AppendColumn(col_sueldo);
	        treeview_reportelista.AppendColumn(col_depto);
	        treeview_reportelista.AppendColumn(col_puesto);
	        treeview_reportelista.AppendColumn(col_jornada); 
	        treeview_reportelista.AppendColumn(col_tipofuncion);
	        treeview_reportelista.AppendColumn(col_tipopago);
	        treeview_reportelista.AppendColumn(col_tipocomida);
	        treeview_reportelista.AppendColumn(col_edad);
		}
		
		enum Column
		{   
		    nom_Empleado,
			col_Empleado,
			col_fecha,
			col_tipo,
			col_sueldo,
			col_puesto,
			col_depto,
			col_jornada,
			col_tipofuncion,
			col_tipopago,
			col_tipocomida,
			col_edad,
		}
		
		enum Column2
		{   
		    col_idEmpleado,
			col_Empleado,
			col_fecha,
			col_causa,
			
		}
		
		void llena_lista_bajas()
		{
		
			fechamin =  Convert.ToDateTime(this.entry_dia_inicio.Text+"/"+this.entry_mes_inicio.Text+"/"+this.entry_anno_inicio.Text).ToShortDateString();
			fechamax =  Convert.ToDateTime(this.entry_dia_fin.Text+"/"+this.entry_mes_fin.Text+"/"+this.entry_anno_fin.Text).ToShortDateString();
				   
			fechaminsql =  this.entry_anno_inicio.Text+"-"+this.entry_mes_inicio.Text+"-"+this.entry_dia_inicio.Text;
			fechamaxsql =  this.entry_anno_fin.Text+"-"+this.entry_mes_fin.Text+"-"+this.entry_dia_fin.Text;
				
			//Console.WriteLine(Convert.ToDateTime(fechamin).ToLongDateString());
			//Console.WriteLine(Convert.ToDateTime(fechamax).ToLongDateString());
			treeViewEngineBusca.Clear();
						
			NpgsqlConnection conexion; 
			conexion = new NpgsqlConnection (connectionString+nombrebd );
            
			// Verifica que la base de datos este conectada
			try{
				conexion.Open ();
				NpgsqlCommand comando; 
				comando = conexion.CreateCommand ();
				//osiris_empleado.id_empleado
             	comando.CommandText = "SELECT osiris_empleado.id_empleado,baja_empleado,nombre1_empleado,nombre2_empleado,apellido_paterno_empleado,apellido_materno_empleado,causa_baja,notas_baja,to_char(fecha_de_baja,'YYYY/MM/dd') as fechadebaja "+
									"FROM osiris_empleado_detalle,osiris_empleado  " +
									"WHERE ((osiris_empleado.id_empleado = osiris_empleado_detalle.id_empleado) "+
									"AND baja_empleado = 'true' "+
									"AND to_char(fecha_de_baja,'yyyy-MM-dd') >= '"+fechaminsql+"' "+
									"AND to_char(fecha_de_baja,'yyyy-MM-dd') <= '"+fechamaxsql+"');";
				//Console.WriteLine(comando.CommandText.ToString());					  
				NpgsqlDataReader lector = comando.ExecuteReader ();
				while (lector.Read()){
                    nombre_completo = ((string) lector["nombre1_empleado"]) + " " + ((string) lector["nombre2_empleado"]) + " " + 
			                          ((string) lector["apellido_paterno_empleado"])+" " +((string) lector["apellido_materno_empleado"]);
			        fechadebaja = ((string) lector["fechadebaja"]);
					treeViewEngineBusca.AppendValues (nombre_completo,(string) lector["id_empleado"],
										Convert.ToDateTime( fechadebaja).ToLongDateString() , (string) lector["causa_baja"]);
				}
				
			}catch (NpgsqlException ex){
	   			MessageDialog msgBoxError = new MessageDialog (MyWinError,DialogFlags.DestroyWithParent,
										MessageType.Error, ButtonsType.Close,"PostgresSQL error: {0}",ex.Message);
				msgBoxError.Run ();	msgBoxError.Destroy();
            }
            conexion.Close ();
		}
		
		
		
		void llena_lista_contrato()
		{
		 
		   fechamin =  Convert.ToDateTime(this.entry_dia_inicio.Text+"/"+this.entry_mes_inicio.Text+"/"+this.entry_anno_inicio.Text).ToShortDateString();
		   fechamax =  Convert.ToDateTime(this.entry_dia_fin.Text+"/"+this.entry_mes_fin.Text+"/"+this.entry_anno_fin.Text).ToShortDateString();
				Console.WriteLine(Convert.ToDateTime(fechamin).ToLongDateString());
		        Console.WriteLine(Convert.ToDateTime(fechamax).ToLongDateString());
			treeViewEngineBusca.Clear();
						
			NpgsqlConnection conexion; 
			conexion = new NpgsqlConnection (connectionString+nombrebd );
            
			// Verifica que la base de datos este conectada
			try{
				conexion.Open ();
				NpgsqlCommand comando; 
				comando = conexion.CreateCommand ();
				
             comando.CommandText = "SELECT id_empleado, historial_de_contrato,nombre1_empleado,nombre2_empleado,apellido_paterno_empleado,apellido_materno_empleado "+
									  "FROM osiris_empleado " +
									 "WHERE ( historial_de_contrato != '');";
									 // nadams consulta los que tienen historial
									  
				NpgsqlDataReader lector = comando.ExecuteReader ();
				
				while (lector.Read())
				   {
				    char[] delimiterChars = {';'};  // delimitador de Cadenas
			        string text = ((string) lector["historial_de_contrato"]); //consulta historial de contrato
			        string[] words = text.Split(delimiterChars);  // Separa las Cadenas
			       // conteo_lineas = 0;
			        id_empleado = (string) lector["id_empleado"];
			        nombre_completo = ((string) lector["nombre1_empleado"]) + " " + ((string) lector["nombre2_empleado"]) + " " + 
			                          ((string) lector["apellido_paterno_empleado"])+" " +((string) lector["apellido_materno_empleado"]);
			        foreach (string s in words){
			        
			       // Console.WriteLine (conteo_lineas);
			        
			        if (s.Length > 0) //comprueba si hay historial de contrato "s" tiene historial
			        {    
			        conteo_lineas = conteo_lineas + 1; //suma los renglones en cada linea
			              
			              
			              switch (conteo_lineas)
                              {
                               case 1: 
                                  //registro
                                 //Console.WriteLine(s);
                               break;
                               case 2:
                                 //fecha de registro
                                 //Console.WriteLine(s);
                               break;
                               case 3:
                                 var_fecha = s;
			                      // if (Convert.ToDateTime (var_fecha) > Convert.ToDateTime(fechamin) && Convert.ToDateTime (var_fecha) < Convert.ToDateTime(fechamax))
                                  //    {fecha_valida = true;}  
                                    if (Convert.ToDateTime (var_fecha) < Convert.ToDateTime(fechamin) || Convert.ToDateTime (var_fecha) > Convert.ToDateTime(fechamax))
                                     {fecha_valida = false;}
                                       else {fecha_valida = true;} 
                               break;
                               case 4:
                                  if (tiempo_de_contrato_short == s || check_todos.Active == true )
                                  {leer = true;}
                                  var_tipo = s;
                                break;
                               case 5:
                                   var_sueldo = s.Trim();
                               break;
                               case 6:
                                   var_depto = s;
                               break;
                               case 7:
                                   var_puesto = s;
                               break;
                               case 8:
                                   var_jornada = s;
                               break;
                               case 9:
                                   var_tipofuncion = s;
                               break;
                               case 10:
                                   var_tipopago = s;
                               break;
                               case 11:
                                   var_tiempocomida = s;
                               break;
                               case 12:
                                   //numero de loker
                               break;
                               case 13:
                                   var_annos = s.Substring(0,s.Length-1);
                                   if (leer == true && fecha_valida == true){
                                   treeViewEngineBusca.AppendValues (nombre_completo,id_empleado, Convert.ToDateTime(var_fecha).ToLongDateString() , var_tipo,var_sueldo ,var_depto,var_puesto,var_jornada,var_tipofuncion, var_tipopago,var_tiempocomida,var_annos);
                                    }
                                   conteo_lineas = 0;
                                   leer = false ;
                                   fecha_valida = false;
                                   
                               break;
                               default:
                                    //Console.WriteLine("Default case");
                               break;
                                }
			        
			      
			           } // X Cierra el if (s > 0.length)
			        
			        } // X Cierra el foreach
				
				}   // X Cierra el while del lector
			
			}  // X Cierra el Try de la conexion
			
			catch (NpgsqlException ex){
	   			MessageDialog msgBoxError = new MessageDialog (MyWinError,DialogFlags.DestroyWithParent,
										MessageType.Error, ButtonsType.Close,"PostgresSQL error: {0}",ex.Message);
				msgBoxError.Run ();	msgBoxError.Destroy();
            } // X Cierra el catch
            conexion.Close ();
		
					} // X Cierra el void				  
			

		void on_selecciona_empleado_clicked(object sender, EventArgs args)
		{
            imprimecontrato();
            
		}
    

        void on_imprimir_clicked(object sender, EventArgs args)
        {
          imprimecontrato();
        }
		
		
		void imprimecontrato()
		{
		
        	TreeModel model;
			TreeIter iterSelected;
			if (treeview_reportelista.Selection.GetSelected(out model, out iterSelected)) {
				id_sel = (string) model.GetValue(iterSelected, 1);
				fechacontr = (string) model.GetValue(iterSelected, 2);
				tpcontrato = 	(string) model.GetValue(iterSelected, 3);		
				tipfun  = 	(string) model.GetValue(iterSelected, 8);		
				depart  = 	(string) model.GetValue(iterSelected, 5);	
				puest  = 	(string) model.GetValue(iterSelected, 6);	
				jorn  = 	(string) model.GetValue(iterSelected, 7);	
				suel  = 	(string) model.GetValue(iterSelected, 4);	
				 tippag  = 	(string) model.GetValue(iterSelected, 9);	
				 tpocomida  = 	(string) model.GetValue(iterSelected, 10);	
				 annosemp  = 	(string) model.GetValue(iterSelected, 11);	
		    }
		    
		    NpgsqlConnection conexion; 
			conexion = new NpgsqlConnection (connectionString+nombrebd );
            
			// Verifica que la base de datos este conectada
			try{
				conexion.Open ();
				NpgsqlCommand comando; 
				comando = conexion.CreateCommand ();				
             	comando.CommandText = "SELECT nombre1_empleado,nombre2_empleado,apellido_paterno_empleado,apellido_materno_empleado,nacionalidad,"+
                                   "calle, numero,colonia "+
									  "FROM osiris_empleado_detalle,osiris_empleado  " +
									"WHERE ((osiris_empleado.id_empleado = osiris_empleado_detalle.id_empleado) and osiris_empleado.id_empleado = '"+id_sel+"' );";
									  
									// "WHERE ( id_empleado = 'id_sel');";
									 // nadams consulta los que tienen historial
									  
				NpgsqlDataReader lector = comando.ExecuteReader ();            
           		if ((bool) lector.Read()){
		
					new rpt_contrato_empleado(tpcontrato,
						  		(string) lector["apellido_paterno_empleado"],
						  		(string) lector["apellido_materno_empleado"],
								(string) lector["nombre1_empleado"],
								(string) lector["nombre2_empleado"],
							   annosemp,
							    (string) lector["calle"],
							    (string) lector["colonia"],
							    (string) lector["numero"],
							    tipfun,
							    depart,
							    puest,
							    jorn,
							    tpocomida,
							    fechacontr,
							    suel,
							    tippag,
							    (string) lector["nacionalidad"]
							    );
							    
				}							    
			}catch (NpgsqlException ex){
	   			MessageDialog msgBoxError = new MessageDialog (MyWinError,DialogFlags.DestroyWithParent,
										MessageType.Error, ButtonsType.Close,"PostgresSQL error: {0}",ex.Message);
				msgBoxError.Run ();	msgBoxError.Destroy();
            }
            conexion.Close ();
		
		}
		
		void on_cierraventanas_clicked (object sender, EventArgs args)
		{
			Widget win = (Widget) sender;
			win.Toplevel.Destroy();
		}
		
	}
	
}
