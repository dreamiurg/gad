This programs helps to download Siebel (Genie) defect/suggestion/inquiry attachments on unstable network connections. Works with Siebel 6.0

Releases
========
1.1
---
* UNC path to where Siebel attachments are stored can now be specified wia command line -r option

1.0
---
* First public release

What it does
============

1. Connects to Siebel (Genie) database. You may specify username/password on the command line. If not set, will use your Windows logon name (will work for most Genie users)
1. Finds information about defect's attachments.
1. Generates and prints to stdout commands to copy and unzip the attachments (``xcopy`` + ``dlls/sseunzip.exe``). You may redirect output to batch file and run it to get attachments

You may add this program to ``PATH`` env variable for your convenience:
``setx path "c:\Program Files\CQG\Genie Attachment Downloader\;%PATH%"``

Usage
=====

Options
-------
The following command line options are supported

    s, server        Database server to connect to (default: sblsdb)
    u, user          Username to connect to DB (default: your Windows logon
                     username)
    p, password      Password to connect to DB (default: same as username)
    d, database      Database to use (default: SiebelDB)
    r, remotepath    UNC path to Siebel attachments (default:
                     \\d8sfs.denver.cqg\GenieAttachments\)
    n, number        Required. Defect/Suggestion/Inquiry number
    v, verbose       Verbose mode
    help             Display this help screen.
  
Sample: download attachments for defect/suggestion/inquiry 226572502
--------------------------------------------------------------------

To download attachments for defect 226572502 into current folder, run:

    gad.exe -n 226572502 > go.bat && go.bat

Sample: batch automation
------------------------ 

You can create fetch.bat that will create new folder by defect number and downloads files there:

    mkdir %1
    gad.exe -n %1 -p 111111 > %1\go.bat && (cd %1 && call go.bat)