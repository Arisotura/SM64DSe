#include "../SM64DS_2.h"

using DLFunc = void(*)();

//@02090864
char* DLLoad(int ov0FileID)
{
	char* file = LoadFile(ov0FileID);
	uint16_t numPtrsToFix = *(uint16_t*)file;
	uint16_t* ptrsOffset = (uint16_t*)(file + *(uint16_t*)(file + 0x02));
	
	for(int i = 0; i < numPtrsToFix; ++i)
	{
		unsigned* ptrToFix = (unsigned*)(file + ptrsOffset[i]);
		
		if(*ptrToFix >> 24 == 0x00)
			*ptrToFix += (unsigned)file;
		
		else if((*ptrToFix & 0x0fffffff) - 0x0a000000 < 0x02000000)
			*ptrToFix = (*ptrToFix & 0xff000000) | ((*ptrToFix & 0x00ffffff) - ((unsigned)ptrToFix / 4) - 2 & 0x00ffffff);
		
		else
			while(true){};//best crash ever
		
	}
	
	//Fixed function calls will appear broken if this is not done.
	InvalidateDataCache(file, ((unsigned*)file)[-3]); //According to SM64DS rules, ((unsigned*)memoryInTheHeap)[-3] is the allocation size
	DLFunc initFunc = (DLFunc)(file + *(uint16_t*)(file + 0x04));
	initFunc();
	
	return file;
}

void DLCleanAllUsedByLevel(unsigned hax)
{
	if(hax != 0x02017f34 || !DL_PTR_ARR_PTR)
		return;
	
	int numDLs = (int)DL_PTR_ARR_PTR[-3] / 4; //According to SM64DS rules, ((unsigned*)memoryInTheHeap)[-3] is the allocation size
	for(int i = 0; i < numDLs; ++i)
	{
		char* file = DL_PTR_ARR_PTR[i];
		DLFunc cleanFunc = (DLFunc)(file + *(uint16_t*)(file + 0x06));
		cleanFunc();
		
		delete file;
	}
	
	delete[] DL_PTR_ARR_PTR;
	DL_PTR_ARR_PTR = nullptr;
}

//The function that leads to this is called 2x per level.
//The difference is the function that r0 points to.
//02017f34 means cleanup.
//02018028 means init.
void DLLoadAllUsedByLevel(unsigned hax)
{
	if(hax != 0x02018028)
		return;
	
	if(DL_PTR_ARR_PTR)
		while(true){}//best crash ever
	
	char* levelData = (char*)0x0214eaa0;
	if(*(unsigned*)(levelData + 0x30) == 0xe12fff1e) //for old levels that didn't get the change yet
		return;
	
	uint16_t* dlData = (uint16_t*)(levelData + *(unsigned*)(levelData + 0x30));
	
	int numDLs = dlData[0];
	if(numDLs == 0)
		return;
	
	char** dlPtrs = new char*[numDLs];
	
	DL_PTR_ARR_PTR = dlPtrs;
	for(int i = 0; i < numDLs; ++i)
		dlPtrs[i] = DLLoad(dlData[i + 1]);
	
}