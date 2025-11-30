' DataManager
'
' Es un simulador que permite definir tipos de datos atomicos, y compuestos los cuales son struct y union,
' y calcular sus tamaños y alineaciones bajo diferentes condiciones.
' Se aplica poliformismo por medio de la clase base TipoBase para manejar
' los diferentes tipos de datos. Para poder representar los tipos compuestos
' struct y union se utilizan listas de tipos que componen dichos tipos que representan
' los campos, y se implementan metodos para calcular los tamanos en formato sin empaquetar, 
' empaquetado y reordenado optimo. Para la union hay que tomar en cuenta que los campos comparten espacio o mejor llamados alternativas, pero en pocas
' palabras eso significa que se reserva es el espacio de la alternativa o campo mas grande. Por tanto como no se comparten espacio 
' no hay padding. Para la alineacion se toma el maximo de los campos que componen el tipo compuesto, de esta forma
' se aplica el modulo para determinar el valor del padding a agregar.
'
' Estudiante: Blanyer Vielma, Carnet: 16-11238

' Imports necesarios
Imports System
Imports System.Collections.Generic
Imports System.Linq

Public Class DataManager

    ' Diccionario para almacenar todos los tipos definidos
    Private ReadOnly tipos As New Dictionary(Of String, TipoBase)

    ' Clase base para utilizar polimorfismo para los tipos atomicos, struct y union
    ' Metodos
    '   CalcularTamanoSinEmpaquetar: Devuelve el tamaño sin empaquetar
    '   CalcularTamanoEmpaquetado: Devuelve el tamaño empaquetado
    '   CalcularTamanoReordenado: Devuelve el tamaño reordenado óptimo
    '   CalcularAlineacion: Devuelve la alineación del tipo
    Public MustInherit Class TipoBase
        Public Property Nombre As String
        Public MustOverride Function CalcularTamanoSinEmpaquetar() As Integer
        Public MustOverride Function CalcularTamanoEmpaquetado() As Integer
        Public MustOverride Function CalcularTamanoReordenado() As Integer
        Public MustOverride Function CalcularAlineacion() As Integer
    End Class

    ' Clase para tipo Atomico
    ' Argumentos:
    '   @nombre: nombre del tipo
    '   @representacion: tamaño en bytes
    '   @alineacion: alineación en bytes
    ' Metodos: 
    '   CalcularTamanoSinEmpaquetar: Devuelve el tamaño sin empaquetar
    '   CalcularTamanoEmpaquetado: Devuelve el tamaño empaquetado
    '   CalcularTamanoReordenado: Devuelve el tamaño reordenado óptimo
    '   CalcularAlineacion: Devuelve la alineación del tipo
    Public Class TipoAtomico
        ' Se heredan los metodos de la clase base
        Inherits TipoBase
        Public Property Representacion As Integer
        Public Property Alineacion As Integer
        ' Constructor para inicializar el tipo atomico
        Public Sub New(nombre As String, representacion As Integer, alineacion As Integer)
            Me.Nombre = nombre
            Me.Representacion = representacion
            Me.Alineacion = alineacion
        End Sub

        ' Se sobrescriben los metodos de la clase base
        Public Overrides Function CalcularTamanoSinEmpaquetar() As Integer
            Return Representacion
        End Function

        Public Overrides Function CalcularTamanoEmpaquetado() As Integer
            Return Representacion
        End Function

        Public Overrides Function CalcularTamanoReordenado() As Integer
            Return Representacion
        End Function

        Public Overrides Function CalcularAlineacion() As Integer
            Return Alineacion
        End Function
    End Class

    ' Tipo struct
    ' Se define una clase para el tipo struct la cual es un tipo compuesto, para poder modelar el struct
    ' se aplica una lista de tipos que son las propiedades o campos del struct.
    ' Sobreescribo los metodos 
    ' Argumentos:
    '   @nombre: nombre del tipo
    '   @campos: lista de tipos que componen el struct
    ' Metodos:
    '   CalcularTamanoSinEmpaquetar: Devuelve el tamaño sin empaquetar
    '   CalcularTamanoEmpaquetado: Devuelve el tamaño empaquetado
    '   CalcularTamanoReordenado: Devuelve el tamaño reordenado óptimo
    '   CalcularAlineacion: Devuelve la alineación del tipo
    Public Class TipoStruct
        ' Se heredan los metodos de la clase base
        Inherits TipoBase
        'Lista de campos del struct
        Public Property Campos As List(Of TipoBase)
        ' Constructor para inicializar el struct
        Public Sub New(nombre As String, campos As List(Of TipoBase))
            Me.Nombre = nombre
            Me.Campos = campos
        End Sub

        Public Overrides Function CalcularTamanoSinEmpaquetar() As Integer
            Dim total = 0
            ' Simplemente sumo los tamaños sin empaquetar de cada campo ya que no hay alineación, es decir, no se reserva espacio extra "padding"
            For Each campo In Campos
                total += campo.CalcularTamanoSinEmpaquetar()
            Next
            Return total
        End Function

        Public Overrides Function CalcularTamanoEmpaquetado() As Integer
            Dim offset = 0 ' Desplazamiento actual
            Dim maxAlineacion = 1 ' Máximo alineamiento encontrado
            For Each campo In Campos
                Dim alineacion = campo.CalcularAlineacion()
                maxAlineacion = Math.Max(maxAlineacion, alineacion)
                ' Ajustar al alineamiento
                If offset Mod alineacion <> 0 Then
                    offset += alineacion - (offset Mod alineacion)
                End If
                offset += campo.CalcularTamanoEmpaquetado()
            Next
            ' Ajustar tamaño final al máximo alineamiento
            If offset Mod maxAlineacion <> 0 Then
                offset += maxAlineacion - (offset Mod maxAlineacion)
            End If
            Return offset
        End Function

        Public Overrides Function CalcularTamanoReordenado() As Integer
            ' Ordenar campos por alineación descendente
            Dim ordenados = Campos.OrderByDescending(Function(c) c.CalcularAlineacion()).ToList()
            Dim offset = 0
            Dim maxAlineacion = 1
            For Each campo In ordenados
                Dim alineacion = campo.CalcularAlineacion()
                maxAlineacion = Math.Max(maxAlineacion, alineacion)
                If offset Mod alineacion <> 0 Then
                    offset += alineacion - (offset Mod alineacion)
                End If
                offset += campo.CalcularTamanoEmpaquetado()
            Next
            If offset Mod maxAlineacion <> 0 Then
                offset += maxAlineacion - (offset Mod maxAlineacion)
            End If
            Return offset
        End Function

        Public Overrides Function CalcularAlineacion() As Integer
            Return Campos.Max(Function(c) c.CalcularAlineacion())
        End Function
    End Class

    ' Tipo union
    ' Se define una clase para el tipo union la cual es un tipo compuesto, para poder modelar la union
    ' se aplica una lista de tipos que son las alternativas de la union
    ' Argumentos:
    '   @nombre: nombre del tipo
    '   @alternativas: lista de tipos que componen la union
    Public Class TipoUnion
        Inherits TipoBase
        Public Property Alternativas As List(Of TipoBase)

        Public Sub New(nombre As String, alternativas As List(Of TipoBase))
            Me.Nombre = nombre
            Me.Alternativas = alternativas
        End Sub

        ' Como se comparte el espacio entre los campos, el tamaño del union es el tamaño del campo más grande
        Public Overrides Function CalcularTamanoSinEmpaquetar() As Integer
            Return Alternativas.Max(Function(c) c.CalcularTamanoSinEmpaquetar())
        End Function
        ' Es lo mismo que sin empaquetar ya que no hay padding
        Public Overrides Function CalcularTamanoEmpaquetado() As Integer
            Return Alternativas.Max(Function(c) c.CalcularTamanoEmpaquetado())
        End Function
        ' El reordenamiento no afecta a las uniones, ya que solo se utiliza el espacio del campo más grande
        Public Overrides Function CalcularTamanoReordenado() As Integer
            Return Alternativas.Max(Function(c) c.CalcularTamanoReordenado())
        End Function
        Public Overrides Function CalcularAlineacion() As Integer
            Return Alternativas.Max(Function(c) c.CalcularAlineacion())
        End Function
    End Class

    ' Definición de tipos
    ' Procedimiento que define un tipo atomico
    ' Argumentos:
    '   @nombre: nombre del tipo
    '   @representacion: tamaño en bytes
    '   @alineacion: alineación en bytes
    Public Sub DefinirAtomico(nombre As String, representacion As Integer, alineacion As Integer)
        tipos(nombre) = New TipoAtomico(nombre, representacion, alineacion)
    End Sub

    ' Definición de struct
    ' Procedimiento que define un tipo struct
    ' Argumentos:
    '   @nombre: nombre del tipo
    '   @campos: lista de nombres de tipos que componen el struct
    Public Sub DefinirStruct(nombre As String, campos As List(Of String))
        Dim listaCampos As New List(Of TipoBase)
        For Each c In campos
            ' Necesitamos verificar que el tipo exista
            If tipos.ContainsKey(c) Then
                listaCampos.Add(tipos(c))
            End If
        Next
        tipos(nombre) = New TipoStruct(nombre, listaCampos)
    End Sub

    ' Definición de union
    ' Procedimiento que define un tipo union
    ' Argumentos:
    '   @nombre: nombre del tipo
    '   @alternativas: lista de nombres de tipos que componen la union
    Public Sub DefinirUnion(nombre As String, alternativas As List(Of String))
        Dim listaAlt As New List(Of TipoBase)
        ' Necesitamos verificar que el tipo exista
        For Each c In alternativas
            If tipos.ContainsKey(c) Then
                listaAlt.Add(tipos(c))
            End If
        Next
        tipos(nombre) = New TipoUnion(nombre, listaAlt)
    End Sub

    ' Describir
    ' Función que devuelve una lista de strings con la descripción del tipo
    ' Argumentos:
    '   @nombre: nombre del tipo a describir
    ' Devuelve:
    '   Una lista de strings con la descripción del tipo donde se indica el tipo, y el peso en bytes 
    '   de la alineacion, sin empaquetar, empaquetado y reordenado óptimo
    Public Function Describir(nombre As String) As List(Of String)
        Dim resultado As New List(Of String)
        ' Necesitamos verificar que el tipo exista
        If Not tipos.ContainsKey(nombre) Then
            resultado.Add($"Error: Tipo '{nombre}' no definido.")
            Return resultado
        End If

        ' Se llanan los metodos para obtener la información del tipo
        Dim t = tipos(nombre)
        resultado.Add($"Tipo: {t.Nombre}")
        resultado.Add($"Alineación: {t.CalcularAlineacion()} bytes")
        resultado.Add($"Sin empaquetar: {t.CalcularTamanoSinEmpaquetar()} bytes")
        resultado.Add($"Empaquetado: {t.CalcularTamanoEmpaquetado()} bytes")
        resultado.Add($"Reordenado óptimo: {t.CalcularTamanoReordenado()} bytes")
        Return resultado
    End Function
End Class

' SimuladorTipos
' Módulo que implementa la interfaz de línea de comandos para interactuar con el DataManager
' Permite definir tipos y describirlos mediante comandos
Module SimuladorDataManager
    Dim manejador As New DataManager()

    ' Interfaz de línea de comandos
    Sub Interfaz()
        While True
            Console.Write("> ")
            Dim comando = Console.ReadLine()
            Dim p = comando.Split(" "c)
            Dim accion = p(0).ToUpper()

            Select Case accion
                Case "ATOMICO"
                    If p.Length = 4 Then
                        manejador.DefinirAtomico(p(1), Integer.Parse(p(2)), Integer.Parse(p(3)))
                    Else
                        Console.WriteLine("Comando ATOMICO inválido.")
                    End If

                Case "STRUCT"
                    If p.Length >= 3 Then
                        manejador.DefinirStruct(p(1), p.Skip(2).ToList())
                    Else
                        Console.WriteLine("Comando STRUCT inválido.")
                    End If

                Case "UNION"
                    If p.Length >= 3 Then
                        manejador.DefinirUnion(p(1), p.Skip(2).ToList())
                    Else
                        Console.WriteLine("Comando UNION inválido.")
                    End If

                Case "DESCRIBIR"
                    If p.Length = 2 Then
                        Dim resultado = manejador.Describir(p(1))
                        For Each linea In resultado
                            Console.WriteLine(linea)
                        Next
                    Else
                        Console.WriteLine("Comando DESCRIBIR inválido.")
                    End If

                Case "SALIR"
                    Exit While
                Case Else
                    Console.WriteLine("Comando no reconocido.")
            End Select
        End While
    End Sub

    ' Punto de entrada del programa
    Sub Main()
        Console.WriteLine("Simulador de DataManager")
        Console.WriteLine("Comandos:")
        Console.WriteLine("  ATOMICO <nombre> <representacion> <alineacion>")
        Console.WriteLine("  STRUCT <nombre> [<tipo>]")
        Console.WriteLine("  UNION <nombre> [<tipo>]")
        Console.WriteLine("  DESCRIBIR <nombre>")
        Console.WriteLine("  SALIR")
        Interfaz()
    End Sub

End Module

