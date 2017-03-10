SET OUTPUT_DIR=App
set SOLUTION_FILE_NAME=Origami.sln

cd "%OUTPUT_DIR%"

REM /m enables parallel builds with MSBuild (see below)
msbuild.exe %SOLUTION_FILE_NAME% /m /t:Build /p:Configuration=Release;Platform=x86;AppxBundle=Always


REM Suggestions to speed up msbuild:
REM 
REM https://superuser.com/questions/44926/building-a-fast-visual-studio-machine
REM 
REM The BIGGEST difference any one component made for us (we tested one thing at a time) was adding really fast I/O and putting all of our source code on that hardware.
REM 
REM The biggest bottleneck is going to be the disk drive when doing builds in Visual Studio.
REM 
REM I use a ramdrive, from Dataram. Visual Studio doesn’t use all my PC cores (8 Core) so what I did was that I format the ramdrive with NTFS and activated Compression. Source coded has high compression ratio so a ramdrive of 2GB gives at least 4GB storage. You can also activate auto save of ramdrive! This method gives you the fastest IO, even faster than SSD. 
REM 
REM 
REM http://stackoverflow.com/questions/6916011/how-do-i-get-windows-to-go-as-fast-as-linux-for-compiling-c
REM 
REM Enable parallel builds with MSBuild; just add the "/m" switch, and it will automatically start one copy of MSBuild per CPU core.
REM 
REM Put your files on an SSD -- helps hugely for random I/O.
REM 
REM If your average file size is much greater than 4KB, consider rebuilding the filesystem with a larger cluster size that corresponds roughly to your average file size.
REM 
REM Make sure the files have been defragmented. Fragmented files cause lots of disk seeks, which can cost you a factor of 40+ in throughput. Use the "contig" utility from sysinternals, or the built-in Windows defragmenter.
REM 
REM If your average file size is small, and the partition you're on is relatively full, it's possible that you are running with a fragmented MFT, which is bad for performance. Also, files smaller than 1K are stored directly in the MFT. The "contig" utility mentioned above can help, or you may need to increase the MFT size. The following command will double it, to 25% of the volume: fsutil behavior set mftzone 2 Change the last number to 3 or 4 to increase the size by additional 12.5% increments. After running the command, reboot and then create the filesystem.
REM 
REM Disable last access time: fsutil behavior set disablelastaccess 1
REM 
REM Disable the indexing service
REM 
REM Disable your anti-virus and anti-spyware software, or at least set the relevant folders to be ignored.
REM 
REM Put your files on a different physical drive from the OS and the paging file. Using a separate physical drive allows Windows to use parallel I/Os to both drives.
REM 
REM Have a look at your compiler flags. The Windows C++ compiler has a ton of options; make sure you're only using the ones you really need.
REM 
REM Try increasing the amount of memory the OS uses for paged-pool buffers (make sure you have enough RAM first): fsutil behavior set memoryusage 2
REM 
REM Check the Windows error log to make sure you aren't experiencing occasional disk errors.
REM 
REM Have a look at Physical Disk related performance counters to see how busy your disks are. High queue lengths or long times per transfer are bad signs.
REM 
REM The first 30% of disk partitions is much faster than the rest of the disk in terms of raw transfer time. Narrower partitions also help minimize seek times.
REM 
REM Are you using RAID? If so, you may need to optimize your choice of RAID type (RAID-5 is bad for write-heavy operations like compiling)
REM 
REM Disable any services that you don't need
REM 
REM Defragment folders: copy all files to another drive (just the files), delete the original files, copy all folders to another drive (just the empty folders), then delete the original folders, defragment the original drive, copy the folder structure back first, then copy the files. 
REM 
REM When Windows builds large folders one file at a time, the folders end up being fragmented and slow. ("contig" should help here, too)
REM 
REM If you are I/O bound and have CPU cycles to spare, try turning disk compression ON. It can provide some significant speedups for highly compressible files (like source code), with some cost in CPU.