.MODEL SMALL
.STACK 100h
.DATA
    ; Bird parts coordinates - updated to match the reference image
    ; Tail (triangle)
    tail_x1 DW 20
    tail_y1 DW 80
    tail_x2 DW 40
    tail_y2 DW 60
    tail_x3 DW 60
    tail_y3 DW 80
    
    ; Body (horizontal line)
    body_x1 DW 60
    body_y1 DW 80
    body_x2 DW 100
    body_y2 DW 80
    
    ; Vertical line
    vert_x1 DW 80
    vert_y1 DW 60
    vert_x2 DW 80
    vert_y2 DW 120
    
    ; Head lines (diagonal lines)
    head_line1_x1 DW 60
    head_line1_y1 DW 60
    head_line1_x2 DW 100
    head_line1_y2 DW 60
    
    head_line2_x1 DW 100
    head_line2_y1 DW 60
    head_line2_x2 DW 120
    head_line2_y2 DW 80
    
    head_line3_x1 DW 60
    head_line3_y1 DW 60
    head_line3_x2 DW 40
    head_line3_y2 DW 80
    
    ; Color settings
    bird_color DB 0Fh  ; White color
    
    ; Exit message
    exit_msg DB 'Press ESC to exit or any other key to continue...$'
    
    ; Head (circle) - adjusted position (unused in current design)
    head_x DW 140
    head_y DW 50
    head_radius DW 20
    
    ; Leg 1 - adjusted (unused in current design)
    leg1_x1 DW 90
    leg1_y1 DW 50
    leg1_x2 DW 70
    leg1_y2 DW 100
    
    ; Leg 2 - adjusted (unused in current design)
    leg2_x1 DW 110
    leg2_y1 DW 50
    leg2_x2 DW 130
    leg2_y2 DW 100
    
    ; Flask/Beaker - new element (unused in current design)
    flask_top_x1 DW 180
    flask_top_y1 DW 50
    flask_top_x2 DW 200
    flask_top_y2 DW 50
    
    flask_left_x1 DW 180
    flask_left_y1 DW 50
    flask_left_x2 DW 170
    flask_left_y2 DW 100
    
    flask_right_x1 DW 200
    flask_right_y1 DW 50
    flask_right_x2 DW 210
    flask_right_y2 DW 100
    
    flask_bottom_x1 DW 170
    flask_bottom_y1 DW 100
    flask_bottom_x2 DW 210
    flask_bottom_y2 DW 100
    
    ; Removed duplicate bird_color definition

.CODE
MAIN PROC
    MOV AX, @DATA
    MOV DS, AX
    
    ; Set video mode (320x200, 256 colors)
    MOV AX, 13h
    INT 10h
    
    ; Clear screen to black
    MOV AX, 0600h
    MOV BH, 00h
    MOV CX, 0000h
    MOV DX, 184Fh
    INT 10h
    
    ; Draw tail (triangle)
    MOV AX, tail_x1
    MOV BX, tail_y1
    MOV CX, tail_x2
    MOV DX, tail_y2
    CALL draw_line
    
    MOV AX, tail_x2
    MOV BX, tail_y2
    MOV CX, tail_x3
    MOV DX, tail_y3
    CALL draw_line
    
    MOV AX, tail_x3
    MOV BX, tail_y3
    MOV CX, tail_x1
    MOV DX, tail_y1
    CALL draw_line
    
    ; Draw body (horizontal line)
    MOV AX, body_x1
    MOV BX, body_y1
    MOV CX, body_x2
    MOV DX, body_y2
    CALL draw_line
    
    ; Draw vertical line
    MOV AX, vert_x1
    MOV BX, vert_y1
    MOV CX, vert_x2
    MOV DX, vert_y2
    CALL draw_line
    
    ; Draw head lines
    MOV AX, head_line1_x1
    MOV BX, head_line1_y1
    MOV CX, head_line1_x2
    MOV DX, head_line1_y2
    CALL draw_line
    
    MOV AX, head_line2_x1
    MOV BX, head_line2_y1
    MOV CX, head_line2_x2
    MOV DX, head_line2_y2
    CALL draw_line
    
    MOV AX, head_line3_x1
    MOV BX, head_line3_y1
    MOV CX, head_line3_x2
    MOV DX, head_line3_y2
    CALL draw_line
    
    ; Wait for key press and check if it's ESC
check_key:
    MOV AH, 0
    INT 16h
    
    ; Check if ESC key (scan code 1) was pressed
    CMP AH, 1
    JE exit_program
    
    ; If not ESC, continue running
    JMP check_key
    
exit_program:
    ; Return to text mode
    MOV AX, 3
    INT 10h
    
    ; Exit program
    MOV AH, 4Ch
    INT 21h
MAIN ENDP

; Draw line procedure using Bresenham's algorithm
; Input: AX = x1, BX = y1, CX = x2, DX = y2
draw_line PROC
    PUSH AX
    PUSH BX
    PUSH CX
    PUSH DX
    PUSH SI
    PUSH DI
    
    MOV SI, AX  ; x1
    MOV DI, BX  ; y1
    
    ; Calculate dx = abs(x2 - x1)
    SUB CX, AX  ; CX = x2 - x1
    JNS dx_positive
    NEG CX      ; CX = abs(CX)
dx_positive:
    
    ; Calculate dy = abs(y2 - y1)
    SUB DX, BX  ; DX = y2 - y1
    MOV BX, DX  ; Save dy in BX
    JNS dy_positive
    NEG BX      ; BX = abs(BX)
dy_positive:
    
    ; Determine which coordinate changes faster
    CMP CX, BX
    JAE x_changes_faster
    
    ; Y changes faster
    MOV AX, SI  ; AX = x1
    MOV SI, DI  ; SI = y1
    MOV DI, AX  ; DI = x1
    
    PUSH CX     ; Swap dx and dy
    MOV CX, BX
    POP BX
    
    MOV AX, 1   ; Default y step
    CMP DX, 0
    JGE y_step_positive1
    MOV AX, -1  ; Negative y step
y_step_positive1:
    PUSH AX     ; Save y step
    
    MOV AX, 1   ; Default x step
    CMP DX, 0
    JGE x_step_positive1
    MOV AX, -1  ; Negative x step
x_step_positive1:
    
    ; Draw the line with y changing faster
    MOV DX, 0   ; Error term
    
y_loop:
    ; Plot the point
    PUSH AX
    PUSH BX
    PUSH CX
    PUSH DX
    
    MOV DX, SI  ; y-coordinate
    MOV CX, DI  ; x-coordinate
    MOV AH, 0Ch ; Function to set pixel
    MOV AL, 0Fh ; White color
    INT 10h
    
    POP DX
    POP CX
    POP BX
    POP AX
    
    ; Update error term and coordinates
    ADD DX, BX
    SUB DX, CX
    JL y_no_change
    
    ADD DI, AX  ; Update x
    ADD DX, CX
    
y_no_change:
    ADD SI, 1   ; Update y
    LOOP y_loop
    
    JMP draw_line_done
    
x_changes_faster:
    MOV AX, 1   ; Default x step
    CMP CX, 0
    JGE x_step_positive2
    MOV AX, -1  ; Negative x step
x_step_positive2:
    PUSH AX     ; Save x step
    
    MOV AX, 1   ; Default y step
    CMP DX, 0
    JGE y_step_positive2
    MOV AX, -1  ; Negative y step
y_step_positive2:
    
    ; Draw the line with x changing faster
    MOV DX, 0   ; Error term
    
x_loop:
    ; Plot the point
    PUSH AX
    PUSH BX
    PUSH CX
    PUSH DX
    
    MOV CX, SI  ; x-coordinate
    MOV DX, DI  ; y-coordinate
    MOV AH, 0Ch ; Function to set pixel
    MOV AL, 0Fh ; White color
    INT 10h
    
    POP DX
    POP CX
    POP BX
    POP AX
    
    ; Update error term and coordinates
    ADD DX, BX
    SUB DX, CX
    JL x_no_change
    
    ADD DI, AX  ; Update y
    ADD DX, CX
    
x_no_change:
    ADD SI, 1   ; Update x
    LOOP x_loop
    
draw_line_done:
    POP DI
    POP SI
    POP DX
    POP CX
    POP BX
    POP AX
    RET
draw_line ENDP

; Draw circle using the direct formula (x^2 + y^2 = r^2)
; Input: CX = center_x, DX = center_y, BX = radius
draw_circle_formula PROC
    PUSH AX
    PUSH BX
    PUSH CX
    PUSH DX
    PUSH SI
    PUSH DI
    
    MOV SI, CX      ; Save center_x in SI
    MOV DI, DX      ; Save center_y in DI
    
    ; Start from -radius to +radius for x
    MOV CX, BX
    NEG CX          ; CX = -radius
    
circle_x_loop:      ; Переименовано с x_loop на circle_x_loop
    ; For each x, calculate y = sqrt(r^2 - x^2)
    PUSH CX
    
    ; Calculate x^2
    MOV AX, CX
    IMUL AX          ; AX = x^2
    
    ; Calculate r^2 - x^2
    PUSH AX
    MOV AX, BX
    IMUL AX          ; AX = r^2
    POP DX
    SUB AX, DX       ; AX = r^2 - x^2
    
    ; Calculate y = sqrt(AX)
    CALL sqrt
    
    ; Plot points for +y and -y
    PUSH CX
    ADD CX, SI       ; CX = center_x + x
    
    PUSH AX
    ADD AX, DI       ; AX = center_y + y
    
    ; Plot (x+cx, y+cy)
    PUSH DX
    MOV DX, AX
    MOV AH, 0Ch
    MOV AL, bird_color
    INT 10h
    POP DX
    
    POP AX
    PUSH AX
    NEG AX
    ADD AX, DI       ; AX = center_y - y
    
    ; Plot (x+cx, -y+cy)
    PUSH DX
    MOV DX, AX
    MOV AH, 0Ch
    MOV AL, bird_color
    INT 10h
    POP DX
    
    POP AX
    POP CX
    
    POP CX
    INC CX
    CMP CX, BX
    JLE circle_x_loop
    
    POP DI
    POP SI
    POP DX
    POP CX
    POP BX
    POP AX
    RET
draw_circle_formula ENDP

; Calculate square root using binary search
; Input: AX = number
; Output: AX = sqrt(number)
sqrt PROC
    PUSH BX
    PUSH CX
    PUSH DX
    
    ; Handle special cases
    CMP AX, 0
    JE sqrt_zero
    
    MOV BX, 0       ; Lower bound
    MOV CX, AX      ; Upper bound
    
sqrt_loop:
    ; Check if we're done
    MOV DX, CX
    SUB DX, BX
    CMP DX, 1
    JLE sqrt_done
    
    ; Calculate mid = (low + high) / 2
    MOV DX, BX
    ADD DX, CX
    SHR DX, 1       ; DX = mid
    
    ; Calculate mid^2
    PUSH AX
    MOV AX, DX
    IMUL AX          ; AX = mid^2
    
    ; Compare with the target number
    MOV BX, AX       ; Сохраняем mid^2 в BX
    POP AX           ; Восстанавливаем исходное число
    
    CMP BX, AX       ; Сравниваем mid^2 с исходным числом
    JA sqrt_too_high
    
    ; mid^2 <= number, so set low = mid
    MOV BX, DX
    JMP sqrt_loop
    
sqrt_too_high:
    ; mid^2 > number, so set high = mid
    MOV CX, DX
    JMP sqrt_loop
    
sqrt_done:
    MOV AX, BX      ; Return the lower bound
    JMP sqrt_exit
    
sqrt_zero:
    MOV AX, 0
    
sqrt_exit:
    POP DX
    POP CX
    POP BX
    RET
sqrt ENDP

END MAIN