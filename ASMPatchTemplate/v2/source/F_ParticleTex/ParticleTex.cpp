#include "../SM64DS_2.h"
//@0208f9d0
namespace Particle
{
	bool Manager::LoadTex(unsigned ov0FileID, unsigned newTexID)
	{
		if(newTexID >= PARTICLE_SYS_TRACKER->manager->numTextures)
			Crash();
		TexDef& texDef = PARTICLE_SYS_TRACKER->manager->texDefArr[newTexID];
		if(texDef.texture)
			return true;
		
		texDef.texture = (Texture*)SharedFilePtr{}.Construct(ov0FileID).Load();
		if(!texDef.texture)
			return false;
		texDef.flags  = texDef.texture->flags;
		texDef.width  = texDef.texture->Width();
		texDef.height = texDef.texture->Height();
		
		Vram::StartTexWrite();
		texDef.texVramOffset = Texture::AllocTexVram(texDef.texture->texelArrSize, texDef.texture->Format() == 5);
		Vram::LoadTex(texDef.texture->TexelArr(), texDef.texVramOffset, texDef.texture->texelArrSize);
		Vram::EndTexWrite();
		Vram::StartPalWrite();
		texDef.palVramOffset = Texture::AllocPalVram(texDef.texture->palleteSize, texDef.texture->Format() == 2);
		Vram::LoadPal(texDef.texture->PalleteColArr(), texDef.palVramOffset, texDef.texture->palleteSize);
		Vram::EndPalWrite();
		
		return true;
	}
	
	void Manager::UnloadNewTexs()
	{
		TexDef* texDefArr = PARTICLE_SYS_TRACKER->manager->texDefArr;
		int numTexDefs = PARTICLE_SYS_TRACKER->manager->numTextures;
		
		for(int i = PARTICLE_SYS_TRACKER->manager->numBuiltInTexs; i < numTexDefs; ++i) if (texDefArr[i].texture)
			delete texDefArr[i].texture;
	}
}