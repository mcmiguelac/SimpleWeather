﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SimpleWeather.Clases
{
    class LecturaXml
    {
        public DatosTiempo getDatosMeteorologicos(XmlReader reader, String dia, String franjaHoraria, String hora)
        {
            DatosTiempo Datos = new DatosTiempo();

            Metodos Metodos = new Metodos();

            using (reader)
            {
                while (reader.Read())
                {
                    if (reader.Name.Equals("dia")) //lee hasta que es un nodo "dia"
                    {
                        reader.MoveToNextAttribute(); //saca el unico atributo que contiene el nodo dia
                        if (reader.Value.Equals(dia)) //comprueba si es igual al dia especificado, sino, salta al siguiente nodo
                        {
                            while (reader.Read())
                            {
                                if (!reader.Name.Equals("dia")) //si encuentra otro nodo dia, no entra y se lo salta entero, 
                                                                //puesto que si llega aqui es por que se ha analizado el nodo dia del dia actual
                                {
                                    if (reader.Name.Equals("prob_precipitacion")) //si es precipitaciones entra y sala los datos
                                    {
                                        if (reader.MoveToNextAttribute())
                                        {
                                            if (reader.Value.Equals(franjaHoraria))
                                            {
                                                reader.Read();
                                                String aux = reader.Value;

                                                if (aux.Equals(""))
                                                {
                                                    Datos.ProbabilidadPrecipitaciones = "0%";
                                                }
                                                else
                                                {
                                                    Datos.ProbabilidadPrecipitaciones = aux + "%";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            reader.Read();
                                            String aux = reader.Value;
                                            if (!aux.Equals("\n\t\t\t"))
                                            {
                                                if (aux.Equals(""))
                                                {
                                                    Datos.ProbabilidadPrecipitaciones = "0%";
                                                }
                                                else
                                                {
                                                    Datos.ProbabilidadPrecipitaciones = aux + "%";
                                                }
                                            }
                                        }
                                    }

                                    if (reader.Name.Equals("estado_cielo")) //si es estado_cielo entra y saca los datos
                                    {
                                        reader.MoveToNextAttribute();
                                        if (reader.Name.Equals("periodo"))
                                        {
                                            if (reader.Value.Equals(franjaHoraria))
                                            {
                                                reader.MoveToNextAttribute();
                                                if (reader.Name.Equals("descripcion"))
                                                {
                                                    String aux = reader.Value;
                                                    if (aux.Equals(""))
                                                    {
                                                        Datos.EstadoCielo = "Sin datos";
                                                    }
                                                    else
                                                    {
                                                        Datos.EstadoCielo = aux;
                                                        reader.Read();
                                                        Datos.CodigoEstadoCielo = reader.Value;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (reader.Name.Equals("descripcion"))
                                            {
                                                String aux = reader.Value;
                                                if (aux.Equals(""))
                                                {
                                                    Datos.EstadoCielo = "Sin datos";
                                                }
                                                else
                                                {
                                                    Datos.EstadoCielo = aux;
                                                    reader.Read();
                                                    Datos.CodigoEstadoCielo = reader.Value;
                                                }
                                            }
                                        }


                                    }

                                    if (reader.Name.Equals("temperatura")) //si es temperatura entra y saca los datos de temperatura maxima, tempretura minima y tempratura actual
                                    {

                                        do
                                        {
                                            reader.Read();
                                            switch (reader.Name)
                                            {
                                                case "maxima":
                                                    if (reader.IsStartElement())
                                                    {
                                                        reader.Read();
                                                        Datos.TemperaturaMaxima = reader.Value + "º";
                                                    }
                                                    break;
                                                case "minima":
                                                    if (reader.IsStartElement())
                                                    {
                                                        reader.Read();
                                                        Datos.TemperaturaMinima = reader.Value + "º";
                                                    }
                                                    break;
                                                case "dato":
                                                    reader.MoveToNextAttribute();
                                                    if (reader.Value.Equals(hora))
                                                    {
                                                        reader.Read();
                                                        Datos.TemperaturaActual = reader.Value + "º";
                                                    }
                                                    break;
                                                default:

                                                    break;
                                            }
                                        } while (!reader.Name.Equals("sens_termica"));
                                    }

                                }
                                else
                                {
                                    reader.Skip(); //si encuentra otro nodo dia, despues de analizar el nodo dia actual, se lo salta entero
                                }
                            }
                        }
                        else
                        {
                            reader.Skip(); //si no es el dia actual se lo salta
                        }
                    }
                }
            }

            #region leer xml entero
            /*
            //leyendo...
            while (reader.Read())
            {
                //definiendo el indentado
                string indentado = new string('\t', reader.Depth);

                //evaluando el tipo de nodo
                switch (reader.NodeType)
                {
                    //tipo de nodo declaración del documento xml.
                    case XmlNodeType.XmlDeclaration:
                        //imprimi valor
                        cad += "<?"+ reader.Value+ "?>"+"\n";
                        break;

                    //tipo de nodo elemento
                    case XmlNodeType.Element:
                        {
                            //si existen atributos.
                            if (reader.HasAttributes)
                            {
                                //cadena que guardará temporalmente los atributos leidos
                                string atributos = null;

                                //leyendo cada uno de los atributos
                                for (int i = 0; i < reader.AttributeCount; i++)
                                {
                                    //usamos el indice para movernos por cada atributo.
                                    reader.MoveToAttribute(i);
                                    //imprimimos, tanto el nombre del atributo, como el valor de este.
                                    atributos += " " + reader.Name + "='" + reader.Value + "'";
                                }

                                //regresamos el puntero al inicio del nodo 
                                //donde se estaba leyendo los atributos.
                                reader.MoveToElement();

                                //evaluamos la profundidad del nodo.
                                if (reader.Depth != 2)
                                {
                                    //si es que el nodo tiene un prefijo
                                    if (reader.Prefix != string.Empty)
                                        //imprimimos.
                                        cad += indentado + "<" + reader.Prefix + ":" + reader.LocalName + atributos + "/>" + "\n";
                                    else
                                        //imprimos pero sin prefijo
                                        cad += indentado + "<" + reader.LocalName + atributos + "/>" + "\n";
                                }
                                //si la profundidad es igual a 2
                                else
                                {
                                    cad += indentado + "<" + reader.LocalName + atributos + "/>" + "\n";
                                }
                            }

                            //si es que no tiene atributos.
                            else
                            {
                                //de nuevo con este rollo de la profundidad.
                                if (reader.Depth != 2)
                                {
                                    //esto es igual a lo de arriba que ya te expliqué.
                                    if (reader.Prefix != string.Empty)
                                        //imprimimos
                                        cad += indentado + "<" + reader.Prefix + ":" + reader.LocalName + ">";

                                    else
                                        //y dale! con la misma pasta, je, je, je...
                                        cad += indentado + "<" + reader.Name + ">";
                                }
                                //en caso de que la profundidad del nodo sea igual a 2.
                                else
                                {
                                    cad += indentado + "<" + reader.Name + ">";
                                }
                            }
                        } break;

                    //ya sabéis para que sirve esto...
                    case XmlNodeType.Comment:
                        cad += indentado + "<!--" + reader.Value + "-->" + "\n";
                        break;

                    //y esto también ya sabéis.
                    case XmlNodeType.Text:
                        cad += reader.Value;
                        break;

                    //y esto más aún... esto en verdad, ya me aburre..
                    case XmlNodeType.EndElement:

                        if (reader.Depth != 2)
                        {
                            if (reader.Prefix != String.Empty)
                                cad += indentado + "</" + reader.Prefix + ":" + reader.LocalName + ">" + "\n";
                            else
                                cad += indentado + "</" + reader.Name + ">" + "\n";
                        }
                        else
                        {
                            cad += reader.Prefix + "</" + reader.Name + ">" + "\n";
                        }
                        break;
                }
            }

            texto.Text = cad;
            */


            /*String cad = "";

            using (reader)
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.IsEmptyElement)
                            cad += "<"+ reader.Name+ "/>";
                        else
                        {
                            cad += "<"+ reader.Name+">";
                            reader.Read(); // Read the start tag.
                            if (reader.IsStartElement())  // Handle nested elements.
                                cad += "\r\n" + reader.Name;
                            cad += reader.Value;  //Read the text content of the element.
                        }
                    }
                } 
            }

             */
            #endregion

            return Datos;
        }
    }
}
