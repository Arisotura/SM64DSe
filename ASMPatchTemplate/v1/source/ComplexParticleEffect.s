.global ComplexParticleEffectForObject

@ Allows for calling the ComplexParticleEffect for an object. 
@ Usage: ComplexParticleEffect(object_address, effect_id, free_four_bytes);
@ Eg.: ComplexParticleEffect(0x12345678, 0x0050, 0x84);

ComplexParticleEffectForObject: 
	push {r4-r9, r14}
	mov r4, r0
	mov r5, r1
	mov r6, r2
	add r0, r13, #0x0C
	add r1, r4, #0x5C
	add r2, r4, #0xA4
	bl OBJ_CalculateObjectNextXYZPostionFromCurrentPositionAndSpeed
	ldr r1, [r13, #0x14]
	mov r0, #0x00
	str r1, [r13]
	str r0, [r13, #0x04]
	str r0, [r13, #0x08]
	ldr r0, [r4, r6]
	ldr r2, [r13, #0x0C]
	ldr r3, [r13, #0x10]
	mov r1, r5
	bl ComplexParticleEffect
	str r0, [r4, r6]
	pop {r4-r9, r14}
	bx r14
