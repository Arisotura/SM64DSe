@ Custom heap setup - for libc
@ for NSMB hacking (and other games too)
@
@ By Dirbaio
@
@ What is this:
@ It basically sets up the libc heap so that 
@ you can use malloc/free (and new and delete from C++) from 
@ your code. Which is quite useful, IMO :D
@
@ It basically reserves some space in RAM and then supplies the correct offsets to libc.

@=======================================
@=======================================

@ This is called right at the boot of the game. Sets fake_heap_start and fake_heap_end
@ BE CAREFUL! Offset might change among games / ROM regions. This one is for NSMB U
@ This code should be called right after the ARM9 binary is decompressed!

repl_020008E4:
	stmfd sp!, {r14}
	
	ldr	r0,=arena_begin	@ newheap base
	ldr	r1,=fake_heap_start
	str	r0,[r1]

	ldr	r0, =arena_end	@ newheap end
	ldr	r1, =fake_heap_end	@ set heap end
	str	r0, [r1]
	
	ldmfd sp!, {r15}


@=======================================

@ This tells the c lib that it should use our heap.
@ It basically replaces function _sbrk_r from libc, which
@ to be honest I dunno exactly what it does, but 
@ this way, it works.

.global _sbrk_r
_sbrk_r:

var_4		= -4

		LDR	R2, =heap_curr
		STR	R4, [SP,#var_4]!
		LDR	R3, [R2]
		CMP	R3, #0
		BEQ	loc_203D7E4

loc_203D7AC:
		LDR	R12, =fake_heap_end
		LDR	R12, [R12]
		CMP	R12, #0
		ADD	R1, R3,	R1
		MOVEQ	R12, SP
		CMP	R12, R1
		MOVCC	R3, #0xC
					;
		STRCC	R3, [R0]
		MOV	R4, SP
		MOVCC	R0, #0xFFFFFFFF
		STRCS	R1, [R2]
		MOVCS	R0, R3
		LDMFD	SP!, {R4}
		BX	LR

loc_203D7E4:
		LDR	R3, =fake_heap_start
		LDR	R3, [R3]
		CMP	R3, #0
		LDREQ	R3, =0x20ACA5C
		STRNE	R3, [R2]
		STREQ	R3, [R2]
		B	loc_203D7AC

heap_curr: .word 0
.pool
@=======================================

@ And, finally, the actual arena space.
@ We just tell the assembler to output a bunch of zero bytes.
@ You can make it bigger or smaller, depending on your needs.
@ Remember that if you make it too big (~100kb) the game
@ will run out of memory and crash.
@ Default is 0x10000 bytes = 64KB

arena_begin:
.fill 0x10000 
arena_end:
	.word 0