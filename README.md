# EXAMEN 3 - LENGUAJES DE PROGRAMACIÓN I
Blanyer Vielma, 16-11238

# Parte Escrita

## 1. Tomando en cuenta X = 2, Y = 3 y Z = 8 evaluamos Li y Ui, donde i ∈ {1, 2, 3}

    L1 = min(X, Y) = min(2, 3) = 2
    L2 = min(X, Z) = min(2, 8) = 2
    L3 = min(Y, Z) = min(3, 8) = 3
    U1 = max(X, Y) = max(2, 3) = 3
    U2 = max(X, Z) = max(2, 8) = 8
    U3 = max(Y, Z) = max(3, 8) = 8

Ahora evaluo las expresiones dadas:

    I = ⌊(L1 + U1)/2⌋ = ⌊(2 + 3)/2⌋ = ⌊2,5⌋ = 2
    J = ⌊(L2 + U2)/2⌋ = ⌊(2 + 8)/2⌋ = ⌊5⌋ = 5
    K = ⌊(L3 + U3)/2⌋ = ⌊(3 + 8)/2⌋ = ⌊5,5⌋ = 5

Considero la expresion dada M : array [L1..U1] of array [L2..U2] of array [L3..U3] of T
que es basicamente  M : array [2..3] of array [2..8] of array [3..8] of T. Una arrya de 
tres dimensiones. Tenemos 3-2+1 = 2, 8-2+1 = 7 y 8-3+ = 6, por tanto es un array de dimension 3
donde tenemos 2x7x6, 84 elementos, como el tamano del tipo T es de 4 bytes tenemos 8 x 84 = 336, es decir
ocupamos 336 bytes.
Donde M inicia. en la direccion cero (0) y que el tamano del tipo T es cuatro (4) busco calcular:

### a) La direccion de M[I][J][K] si las matrices se guardan en row–major.

La direccion que tenemos es M[I][J][K] = M[2][5][5] 

    S3 = 4
    S2 = (U3 - L3 + 1)XS3 = (8 - 3 + 1)X4 = 6X4 = 24
    S1 = (U2 - L2 + 1)XS2 = (8 - 2 + 1)X24 = 7X24 = 168
    Direccion = Inicia + ((I - L1)XS1) + ((J - L2)XS2) + ((K - L3)XS3)
    Direccion = 0 + ((2 - 2)X168) + ((5 - 2)X24) + ((5 - 3)X4)
    Direccion = 0 + (0) + (3X24) + (2X4)
    Direccion = 0 + 0 + 72 + 8 = 80

### b) La dirección de M[I][J][K] si las matrices se guardan en column–major

Igual que el inciso a busco M[2][5][5], solo cambio S1 POR S3. 

    S1 = 4
    S2 = (U1 - L1 + 1)XS1 = (3 - 2 + 1)X4 = 2X4 = 8
    S3 = (U2 - L2 + 1)XS2 = (8 - 2 + 1)X8 = 7X8 = 56
    Direccion = Inicia + ((I - L1)XS1) + ((J - L2)XS2) + ((K - L3)XS3)
    Direccion = 0 + ((2 - 2)X4) + ((5 - 2)X8) + ((5 - 3)X56)
    Direccion = 0 + (0) + (3X8) + (2X56)
    Direccion = 0 + 0 + 24 + 112 = 136
