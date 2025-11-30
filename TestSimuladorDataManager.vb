Imports System.IO
Imports SimuladorDataManager
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass>
Public Class TestSimuladorDataManager
    ' Prueba tipos atómicos y Describir
    <TestMethod>
    Public Sub TestAtomico()
        Dim manejador As New DataManager()
        manejador.DefinirAtomico("int", 4, 4)

        Dim resultado = manejador.Describir("int")
        Dim texto = String.Join(Environment.NewLine, resultado)

        Assert.IsTrue(texto.Contains("Tipo: int"))
        Assert.IsTrue(texto.Contains("Alineación: 4 bytes"))
        Assert.IsTrue(texto.Contains("Sin empaquetar: 4 bytes"))
        Assert.IsTrue(texto.Contains("Empaquetado: 4 bytes"))
        Assert.IsTrue(texto.Contains("Reordenado óptimo: 4 bytes"))
    End Sub

    ' Prueba struct donde el reordenado mejora el empaquetado
    <TestMethod>
    Public Sub TestStruct_ReordenadoMejora()
        Dim m As New DataManager()
        m.DefinirAtomico("A", 1, 1)  ' 1 byte, align 1
        m.DefinirAtomico("C", 2, 2)  ' 2 bytes, align 2
        m.DefinirAtomico("B", 1, 1)  ' 1 byte, align 1

        m.DefinirStruct("S", New List(Of String) From {"A", "C", "B"})

        Dim desc = m.Describir("S")
        Dim texto = String.Join(Environment.NewLine, desc)

        ' Suma sin empaquetar = 1 + 2 + 1 = 4
        Assert.IsTrue(texto.Contains("Sin empaquetar: 4 bytes"))

        ' Empaquetado en el orden dado (esperado: 6 bytes por padding)
        Assert.IsTrue(texto.Contains("Empaquetado: 6 bytes"))

        ' Reordenado óptimo debe eliminar padding y dejar 4 bytes
        Assert.IsTrue(texto.Contains("Reordenado óptimo: 4 bytes"))
    End Sub

    ' Prueba union tamaño y alineación deben ser el máximo entre alternativas
    <TestMethod>
    Public Sub TestUnion()
        Dim m As New DataManager()
        m.DefinirAtomico("x", 4, 4)
        m.DefinirAtomico("y", 8, 8)
        m.DefinirUnion("U", New List(Of String) From {"x", "y"})

        Dim desc = m.Describir("U")
        Dim texto = String.Join(Environment.NewLine, desc)

        Assert.IsTrue(texto.Contains("Tipo: U"))
        Assert.IsTrue(texto.Contains("Alineación: 8 bytes"))
        Assert.IsTrue(texto.Contains("Sin empaquetar: 8 bytes"))
        Assert.IsTrue(texto.Contains("Empaquetado: 8 bytes"))
        Assert.IsTrue(texto.Contains("Reordenado óptimo: 8 bytes"))
    End Sub

    ' Describir tipo no definido
    <TestMethod>
    Public Sub TestDescribirIndefinido()
        Dim m As New DataManager()
        Dim resultado = m.Describir("NoExiste")
        Assert.IsTrue(resultado.Count = 1)
        Assert.IsTrue(resultado(0).Contains("Error: Tipo 'NoExiste' no definido."))
    End Sub

    ' Prueba struct simple con varios tamaños y comprobación general
    <TestMethod>
    Public Sub TestStructEmpaquetadoBasico()
        Dim m As New DataManager()
        m.DefinirAtomico("a", 1, 1)
        m.DefinirAtomico("b", 4, 4)

        m.DefinirStruct("T", New List(Of String) From {"a", "b"})

        Dim desc = m.Describir("T")
        Dim texto = String.Join(Environment.NewLine, desc)

        Assert.IsTrue(texto.Contains("Alineación: 4 bytes"))
        Assert.IsTrue(texto.Contains("Sin empaquetar: 5 bytes"))
        Assert.IsTrue(texto.Contains("Empaquetado: 8 bytes"))
    End Sub

End Class

