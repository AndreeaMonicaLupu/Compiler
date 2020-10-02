mov r1 1
mov r2 3
et: dec r1
bne et
mov r2 2(r1)
or r2 0
halt