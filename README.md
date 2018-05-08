# IM-Destinations-Creator
IM-Destinations-Creator is a tool for building and editing IM Destinations files for a future version of FYM. Download FYM from the website: http://fymanager.co.uk/

## Building IM-Destinations-Creator
You will need WindowsAPICodePack, available on NuGet. IM-Destinations-Creator is built on VS2017 Community Edition. You will also need FYMFileZip.dll which is included with FYM. 
Alternatively, you can obtain a compiled executable from the Releases page here.

## Setup and Usage 
Place IMDCreator.exe in the same folder as FYManager.exe. On running, IMDCreator will look for existing yards in the FYMYards.zip file. If it cannot find the file it will ask for another file. If no FYMYards.zip file is provided, the editor will not have any predefined yard names. 

You can add new yards to the left hand column from Edit->Add New Yard. It is not currently possible to rename yards added. 

You can add destinations by selecting a start yard from the left hand column, then selecting as many destinations as you prefer from the centre column (Ctrl and Shift allow selecting multiple entries). Click the right arrow button next to the right hand column to move those yards to the right hand column. The right hand column shows all the current destinations for the currently selected starting yard (in the left hand column). 

You can remove a destination by selecting it in the right hand column, then clicking the left arrow button next to the right hand column. You can remove all destinations by selecting File->New, or by pressing Ctrl + N. 

You can save the selected set of destinations to file with File->Save or by pressing Ctrl + S. It is recommended to save often, as there is presently no undo/redo command.

Finally, you can open previously saved .imd files with File-Open or by pressing Ctrl + O.

## Licensing
IM Destinations Creator
Copyright (C) 2018  Bill 'Blu3wolf' Teale

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program. If not, see <http://www.gnu.org/licenses/>.
