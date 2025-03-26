.MODEL SMALL
.STACK 100h
.DATA
    input_buffer DB 20, ?, 20 DUP(0)
    num1 DW 0
    num2 DW 0
    operation DB 0
    result DW 0
    prompt1 DB 'Enter first number: $'
    prompt2 DB 'Enter second number: $'
    prompt_op DB 'Enter operation (+, -, *, /): $'
    result_msg DB 'Result: $'
    result_buffer DB 6 DUP(0)
    error_msg DB 'Error: Invalid operation or division by zero.$'

.CODE
MAIN PROC
    MOV AX, @DATA
    MOV DS, AX

    CALL clear_buffer
    MOV AH, 09h
    LEA DX, prompt1
    INT 21h

    MOV AH, 0Ah
    LEA DX, input_buffer
    INT 21h

    LEA SI, input_buffer + 2
    CALL build_number
    MOV num1, AX

    CALL new_line

    CALL clear_buffer
    MOV AH, 09h
    LEA DX, prompt2
    INT 21h

    MOV AH, 0Ah
    LEA DX, input_buffer
    INT 21h

    LEA SI, input_buffer + 2
    CALL build_number
    MOV num2, AX

    CALL new_line

    CALL clear_buffer
    MOV AH, 09h
    LEA DX, prompt_op
    INT 21h

    MOV AH, 01h
    INT 21h
    MOV operation, AL

    CALL new_line

    MOV AX, num1
    CMP operation, '+'
    JE do_add
    CMP operation, '-'
    JE do_sub
    CMP operation, '*'
    JE do_mul
    CMP operation, '/'
    JE do_div
    JMP error

do_add: ADD AX, num2
    JMP store_result

do_sub: SUB AX, num2
    JMP store_result

do_mul: MUL num2
    JMP store_result

do_div: CMP num2, 0
    JE error
    XOR DX, DX
    DIV num2
    JMP store_result

store_result:
    MOV result, AX

    CMP AX, 0
    JGE positive
    MOV AH, 02h
    MOV DL, '-'
    INT 21h
    NEG AX

positive:
    LEA DI, result_buffer + 5
    MOV CX, 0
convert_loop:
    MOV DX, 0
    MOV BX, 10
    DIV BX
    ADD DL, '0'
    MOV [DI], DL
    DEC DI
    INC CX
    CMP AX, 0
    JNE convert_loop

    CMP CX, 0
    JNE print_digits
    MOV [DI], '0'
    INC CX
    DEC DI

print_digits:
    INC DI
    MOV AH, 09h
    LEA DX, result_msg 
    INT 21h

    MOV AH, 02h
print_loop:
    MOV DL, [DI]
    INT 21h
    INC DI
    LOOP print_loop

    CALL new_line

    MOV AH, 4Ch
    INT 21h

error:
    MOV AH, 09h
    LEA DX, error_msg
    INT 21h
    MOV AH, 4Ch
    INT 21h

MAIN ENDP

skip_spaces PROC
skip_loop:
    MOV AL, [SI]
    CMP AL, ' '
    JNE done_skip
    INC SI
    JMP skip_loop
done_skip:
    RET
skip_spaces ENDP

build_number PROC
    MOV AX, 0
build_loop:
    MOV BL, [SI]
    CMP BL, '0'
    JB done_build
    CMP BL, '9'
    JA done_build
    SUB BL, '0'
    MOV CX, 10
    MUL CX
    ADD AX, BX
    INC SI
    JMP build_loop
done_build:
    RET
build_number ENDP

clear_buffer PROC
    MOV CX, 20
    LEA DI, input_buffer + 2
    MOV AL, 0
clear_loop:
    MOV [DI], AL
    INC DI
    LOOP clear_loop
    MOV input_buffer + 1, 0
    RET
clear_buffer ENDP

new_line PROC
    MOV AH, 02h
    MOV DL, 0Dh
    INT 21h
    MOV DL, 0Ah
    INT 21h
    RET
new_line ENDP

END MAIN
