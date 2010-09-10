// rpt_envio_de_facturas.cs created with MonoDevelop
// User: ipena at 08:56 a 27/06/2008
// R. Israel Peña Gzz.
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using Npgsql;
using System.Data;
using Gtk;
using Glade;
using Gnome;
using System.Collections;

namespace osiris
{
	public class rpt_envio_de_facturas
	{
		string connectionString;		
		string nombrebd;
		string query_facturas = "" ;
		
		Gtk.TreeView treeview_lista_facturas;
		Gtk.TreeStore treeViewEngineBuscafacturas;
		
		//public string check_todos_clientes;//Gtk.CheckButton check_todos_clientes;	
	    
		string entry_al_dia;
		string entry_al_mes;
		string entry_al_anno;
		string entry_del_dia;
		string entry_del_mes;
		string entry_del_anno;
		string entry_buscar;
		
		string pagado;
		int numerofactura;
		string totalfactura;
		string fechadefectura;
		int foliodeservicio;
		string nombrepaciente;
		string fecha_de_envio;
		string descripcioncliente;
		decimal total_facturas_enviadas = 1;
		decimal total_monto_facturas;
		
		string nombrepacienterecortado;
		string descripcionclienterecortado;
		
		string titulo = "REPORTE ENVIO DE FACTURAS";
		
		int contador = 1;
		int numpage = 1;
		int filas = 730;
		
		//Declaracion de ventana de error:
		protected Gtk.Window MyWinError;
		protected Gtk.Window MyWin;
		
		class_conexion conexion_a_DB = new class_conexion();
		
		public rpt_envio_de_facturas(decimal total_monto_facturas_,string fecha_de_envio_,string entry_buscar_,string entry_al_dia_,string entry_al_mes_,string entry_al_anno_,string entry_del_dia_,string entry_del_mes_,string entry_del_anno_,object _treeview_lista_facturas_,object _treeViewEngineBuscafacturas_,string query_facturas_,string nombrebd_)
		{
		    total_monto_facturas = total_monto_facturas_;
			fecha_de_envio = fecha_de_envio_;
			entry_buscar = entry_buscar_;	
			entry_al_dia = entry_al_dia_;
			entry_al_mes = entry_al_mes_;
            entry_al_anno = entry_al_anno_;
            entry_del_dia = entry_del_dia_;
            entry_del_mes = entry_del_mes_;
            entry_del_anno = entry_del_anno_;	
            treeview_lista_facturas = _treeview_lista_facturas_ as Gtk.TreeView;
            treeViewEngineBuscafacturas = _treeViewEngineBuscafacturas_ as Gtk.TreeStore;	
            query_facturas = query_facturas_;
            connectionString = conexion_a_DB._url_servidor+conexion_a_DB._port_DB+conexion_a_DB._usuario_DB+conexion_a_DB._passwrd_user_DB;
			nombrebd = conexion_a_DB._nombrebd;
          
			Gnome.PrintJob    trabajo   = new Gnome.PrintJob (PrintConfig.Default ());
			Gnome.PrintDialog dialogo   = new Gnome.PrintDialog (trabajo, titulo, 0);
			int respuesta = dialogo.Run ();
			       	            
			if (respuesta == (int) PrintButtons.Cancel){  //boton Cancelar
				dialogo.Hide (); 
				dialogo.Dispose (); 
				return;
			}
			Gnome.PrintContext ctx = trabajo.Context;
			ComponerPagina(ctx, trabajo); 
			trabajo.Close();
			switch (respuesta){  //imprimir
			case (int) PrintButtons.Print:   
				trabajo.Print (); 
				break;
					    
			case (int) PrintButtons.Preview:
				new PrintJobPreview(trabajo, titulo).Show();
				break;
			}
			 dialogo.Hide (); dialogo.Dispose ();
			}  
		void ComponerPagina (Gnome.PrintContext ContextoImp, Gnome.PrintJob trabajoImpresion)
		{   
			// Crear una fuente 
			Gnome.Font fuente = Gnome.Font.FindClosest("Bitstream Vera Sans", 12);
			Gnome.Font fuente1 = Gnome.Font.FindClosest("Bitstream Vera Sans", 6);
			Gnome.Font fuente2 = Gnome.Font.FindClosest("Bitstream Vera Sans", 8);
			//Gnome.Font fuente3 = Gnome.Font.FindClosest("Bitstream Vera Sans", 7);
			Gnome.Font fuente4 = Gnome.Font.FindClosest("Bitstream Vera Sans", 10);
			Gnome.Font fuente5 = Gnome.Font.FindClosest("Bitstream Vera Sans", 9);
				   
			contador = 0;
			numpage = 1;
				
			ContextoImp.BeginPage("Pagina "+numpage.ToString());
			imprime_encabezado(ContextoImp,trabajoImpresion);
			
			//Encabezado de pagina
				
			NpgsqlConnection conexion; 
			conexion = new NpgsqlConnection (connectionString+nombrebd );
	            
			// Verifica que la base de datos este conectada
			try{
				conexion.Open ();
				NpgsqlCommand comando; 
				comando = conexion.CreateCommand ();
														
				comando.CommandText = query_facturas;
					
				NpgsqlDataReader lector = comando.ExecuteReader ();
				         
				while (lector.Read()){
					//Console.WriteLine(" entre");
					numerofactura = (int) lector["numero_factura"];
					totalfactura = (string) lector["totalfactura"].ToString().Trim();
					fechadefectura = (string) lector["fechadefectura"];
					foliodeservicio = (int) lector["folio_de_servicio"];
					nombrepaciente = (string) lector["nombre1_paciente"] + " " + lector["nombre2_paciente"] + " "+ lector["apellido_paterno_paciente"] + " " +  lector["apellido_materno_paciente"];
					fecha_de_envio = (string) lector["fechadeenvio"];
					descripcioncliente = (string) lector["descripcion_cliente"]; 
					pagado = Convert.ToString((bool) lector["pagado"]);
					//Console.WriteLine(numerofactura); 
					if(descripcioncliente.Length > 27){
						descripcionclienterecortado = descripcioncliente.Substring(0,27); 
					}else{
						descripcionclienterecortado = descripcioncliente;
					}
					if (nombrepaciente.Length > 27){
						nombrepacienterecortado = nombrepaciente.Substring(0,27); 
					}else{
						nombrepacienterecortado = nombrepaciente;
					}
					Gnome.Print.Setfont(ContextoImp,fuente1);
	                ContextoImp.MoveTo(33, filas);		ContextoImp.Show(numerofactura.ToString());
					ContextoImp.MoveTo(110, filas);		ContextoImp.Show(totalfactura);
					ContextoImp.MoveTo(65, filas);		ContextoImp.Show(fechadefectura);
					ContextoImp.MoveTo(155, filas);		ContextoImp.Show(foliodeservicio.ToString());
					ContextoImp.MoveTo(195, filas);		ContextoImp.Show(fecha_de_envio);
					ContextoImp.MoveTo(275, filas);		ContextoImp.Show(nombrepacienterecortado);
					ContextoImp.MoveTo(245, filas);		ContextoImp.Show(pagado);
			        ContextoImp.MoveTo(400, filas);		ContextoImp.Show(descripcionclienterecortado);
					ContextoImp.MoveTo(20, filas);		ContextoImp.Show(total_facturas_enviadas.ToString());		
					contador += 1;
					total_facturas_enviadas += 1;
					filas -= 10;
					
					salto_pagina(ContextoImp,trabajoImpresion);
				}
				
				Gnome.Print.Setfont(ContextoImp,fuente4);
				ContextoImp.MoveTo(20, 70);            ContextoImp.Show("TOTAL FACTURAS ENVIADAS:");
				ContextoImp.MoveTo(20.5, 70);            ContextoImp.Show("TOTAL FACTURAS ENVIADAS:");
				ContextoImp.MoveTo(360, 70);           ContextoImp.Show("TOTAL MONTO FACTURAS:");
				ContextoImp.MoveTo(360.5, 70);           ContextoImp.Show("TOTAL MONTO FACTURAS:");
				Gnome.Print.Setfont(ContextoImp,fuente5);
				ContextoImp.MoveTo(170, 70);            ContextoImp.Show(total_facturas_enviadas.ToString());
				ContextoImp.MoveTo(495, 70);           ContextoImp.Show(total_monto_facturas.ToString());
				    
				    
				Gnome.Print.Setfont(ContextoImp,fuente);
				ContextoImp.MoveTo(270, 744);           ContextoImp.Show( titulo+"");	
				ContextoImp.MoveTo(270.5, 744);           ContextoImp.Show( titulo+"");
				
				Gnome.Print.Setfont(ContextoImp,fuente2);
				ContextoImp.MoveTo(300, 758);            ContextoImp.Show("SISTEMA HOSPITALARIO OSIRIS");
			    ContextoImp.MoveTo(20, 752);            ContextoImp.Show("SISTEMA HOSPITALARIO OSIRIS");
				ContextoImp.MoveTo(20, 744);            ContextoImp.Show("Direccion:");
				ContextoImp.MoveTo(20, 736);            ContextoImp.Show("Conmutador:");
				ContextoImp.MoveTo(340, 730);			ContextoImp.Show("PAGINA "+numpage);
				ContextoImp.MoveTo(340.5, 730);			ContextoImp.Show("PAGINA ");
					
				Gnome.Print.Setfont(ContextoImp,fuente4);
			    ContextoImp.MoveTo(60, 720);			ContextoImp.Show(entry_buscar);
				ContextoImp.MoveTo(60.5, 720);			ContextoImp.Show(entry_buscar);
			    ContextoImp.MoveTo(455, 720);			ContextoImp.Show(fecha_de_envio);
				ContextoImp.MoveTo(455.5, 720);			ContextoImp.Show(fecha_de_envio);
				   
				Gnome.Print.Setfont(ContextoImp,fuente4);
				ContextoImp.MoveTo(20, 720);			ContextoImp.Show("Cliente:");
				ContextoImp.MoveTo(20.5, 720);			ContextoImp.Show("Cliente:");
				ContextoImp.MoveTo(390, 720);			ContextoImp.Show("Fecha Envio:");
				ContextoImp.MoveTo(390.5, 720);			ContextoImp.Show("Fecha Envio:");
				ContextoImp.MoveTo(390, 710);			ContextoImp.Show("Fecha Impresion: "+DateTime.Now.ToString("dd-MM-yyyy HH:mm"));
				ContextoImp.MoveTo(390.5, 710);			ContextoImp.Show("Fecha Impresion:");
				
				Gnome.Print.Setfont(ContextoImp,fuente1);
				ContextoImp.MoveTo(20, 700);            ContextoImp.Show("Nº de Factura:");
				ContextoImp.MoveTo(20.5, 700);          ContextoImp.Show("Nº de Factura:");
				ContextoImp.MoveTo(65, 700);			ContextoImp.Show("Fecha Factura:");
				ContextoImp.MoveTo(65.5, 700);			ContextoImp.Show("Fecha Factura:");
				ContextoImp.MoveTo(110, 700);			ContextoImp.Show("Monto Factura:");
				ContextoImp.MoveTo(110.5, 700);			ContextoImp.Show("Monto Factura:");
				ContextoImp.MoveTo(155, 700);			ContextoImp.Show("No. Atencion:");
				ContextoImp.MoveTo(155.5, 700);			ContextoImp.Show("No. Atencion:");
				ContextoImp.MoveTo(195, 700);			ContextoImp.Show("Fecha de Envio:");
				ContextoImp.MoveTo(195.5, 700);			ContextoImp.Show("Fecha de Envio:");
				ContextoImp.MoveTo(245, 700);			ContextoImp.Show("Pagado:");
				ContextoImp.MoveTo(245.5, 700);			ContextoImp.Show("Pagado:");
				ContextoImp.MoveTo(275, 700);			ContextoImp.Show("Paciente:");
				ContextoImp.MoveTo(275.5, 700);			ContextoImp.Show("Paciente:");
				ContextoImp.MoveTo(400, 700);			ContextoImp.Show("Cliente:");
				ContextoImp.MoveTo(400.5, 700);			ContextoImp.Show("Cliente:");
				filas = 750;
				ContextoImp.ShowPage();	
					
			}catch (NpgsqlException ex){
				MessageDialog msgBoxError = new MessageDialog (MyWinError,DialogFlags.DestroyWithParent,
				                                               MessageType.Error, 
				                                               ButtonsType.Close,"PostgresSQL error: {0}",ex.Message);
				msgBoxError.Run ();		msgBoxError.Destroy();
			}
			conexion.Close();
		}
		
		void imprime_encabezado(Gnome.PrintContext ContextoImp, Gnome.PrintJob trabajoImpresion)
		{        		
      		Gnome.Font fuente = Gnome.Font.FindClosest("Bitstream Vera Sans", 12);
			Gnome.Font fuente1 = Gnome.Font.FindClosest("Bitstream Vera Sans", 6);
			Gnome.Font fuente2 = Gnome.Font.FindClosest("Bitstream Vera Sans", 8);
			Gnome.Font fuente3 = Gnome.Font.FindClosest("Bitstream Vera Sans", 7);
			Gnome.Font fuente4 = Gnome.Font.FindClosest("Bitstream Vera Sans", 10);
			Gnome.Font fuente5 = Gnome.Font.FindClosest("Bitstream Vera Sans", 9);
			Gnome.Print.Setfont(ContextoImp,fuente);
			ContextoImp.MoveTo(270, 744);           ContextoImp.Show( titulo+"");	
			ContextoImp.MoveTo(270.5, 744);           ContextoImp.Show( titulo+"");
				
			Gnome.Print.Setfont(ContextoImp,fuente2);
			ContextoImp.MoveTo(300, 758);            ContextoImp.Show("SISTEMA HOSPITALARIO OSIRIS");
			ContextoImp.MoveTo(20, 752);            ContextoImp.Show("SISTEMA HOSPITALARIO OSIRIS");
			ContextoImp.MoveTo(20, 744);            ContextoImp.Show("Direccion:");
			ContextoImp.MoveTo(20, 736);            ContextoImp.Show("Conmutador:");
			ContextoImp.MoveTo(340, 730);			ContextoImp.Show("PAGINA "+numpage);
			ContextoImp.MoveTo(340.5, 730);			ContextoImp.Show("PAGINA ");
					
			Gnome.Print.Setfont(ContextoImp,fuente4);
			ContextoImp.MoveTo(60, 720);			ContextoImp.Show(entry_buscar);
			ContextoImp.MoveTo(60.5, 720);			ContextoImp.Show(entry_buscar);
			ContextoImp.MoveTo(455, 720);			ContextoImp.Show(fecha_de_envio);
			ContextoImp.MoveTo(455.5, 720);			ContextoImp.Show(fecha_de_envio);
				   
			Gnome.Print.Setfont(ContextoImp,fuente4);
			ContextoImp.MoveTo(20, 720);			ContextoImp.Show("Cliente:");
			ContextoImp.MoveTo(20.5, 720);			ContextoImp.Show("Cliente:");
			ContextoImp.MoveTo(390, 720);			ContextoImp.Show("Fecha Envio:");
			ContextoImp.MoveTo(390.5, 720);			ContextoImp.Show("Fecha Envio:");
			ContextoImp.MoveTo(390, 710);			ContextoImp.Show("Fecha Impresion: "+DateTime.Now.ToString("dd-MM-yyyy HH:mm"));
			ContextoImp.MoveTo(390.5, 710);			ContextoImp.Show("Fecha Impresion:");
				
			Gnome.Print.Setfont(ContextoImp,fuente1);
			ContextoImp.MoveTo(20, 700);            ContextoImp.Show("Nº de Factura:");
			ContextoImp.MoveTo(20.5, 700);          ContextoImp.Show("Nº de Factura:");
			ContextoImp.MoveTo(65, 700);			ContextoImp.Show("Fecha Factura:");
			ContextoImp.MoveTo(65.5, 700);			ContextoImp.Show("Fecha Factura:");
			ContextoImp.MoveTo(110, 700);			ContextoImp.Show("Monto Factura:");
			ContextoImp.MoveTo(110.5, 700);			ContextoImp.Show("Monto Factura:");
			ContextoImp.MoveTo(155, 700);			ContextoImp.Show("No. Atencion:");
			ContextoImp.MoveTo(155.5, 700);			ContextoImp.Show("No. Atencion:");
			ContextoImp.MoveTo(195, 700);			ContextoImp.Show("Fecha de Envio:");
			ContextoImp.MoveTo(195.5, 700);			ContextoImp.Show("Fecha de Envio:");
			ContextoImp.MoveTo(245, 700);			ContextoImp.Show("Pagado:");
			ContextoImp.MoveTo(245.5, 700);			ContextoImp.Show("Pagado:");
			ContextoImp.MoveTo(275, 700);			ContextoImp.Show("Paciente:");
			ContextoImp.MoveTo(275.5, 700);			ContextoImp.Show("Paciente:");
			ContextoImp.MoveTo(400, 700);			ContextoImp.Show("Cliente:");
			ContextoImp.MoveTo(400.5, 700);			ContextoImp.Show("Cliente:");
			filas=690;
		}
		
		void salto_pagina(Gnome.PrintContext ContextoImp, Gnome.PrintJob trabajoImpresion)
		{			
			if (contador > 60 ){
				numpage +=1;
				filas = 750;
				contador = 0;
				
				ContextoImp.ShowPage();	
				ContextoImp.BeginPage("Pagina "+numpage.ToString());
				
				imprime_encabezado(ContextoImp,trabajoImpresion);
				
			}
	    }
	}
}

