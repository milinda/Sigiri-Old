WSO2 Carbon ${product.version}
-----------------

${buildNumber}

Welcome to the WSO2 Carbon ${product.version} release

WSO2 Carbon is the base platform for all WSO2 products, powered by OSGi.  It is a
lightweight, high performing platform which is a collection of OSGi bundles. All
the major features which are included in WSO2 products have been developed as
pluggable Carbon components and installed into this base Carbon platform.

What's New In This Release
----------------------------
1. Kerberos support for securing services
2. ApacheDS embedded LDAP
3. RememberMe feature
4. SAML2 relying party components
5. WS-Trust client side token validation
6. Read/Write LDAP realm
7. Various bug fixes (https://wso2.org/jira/secure/IssueNavigator.jspa?mode=hide&requestId=10527) and
   enhancements including stabilizing WSO2 Carbon.

Key Features
------------
1. Provisioning support through p2 - The Feature Manager which comes with
   WSO2 Carbon can be used to install any Carbon feature (A collection of OSGi
   bundles) into Carbon core. This can be used to create a customized Carbon
   based product according to user requirements.
2. Carbon UI framework – The base framework for UI bundles.
3. User management – Management of users and roles are completely handled
   by Carbon core.
4. Registry – The Registry API comes from Carbon core.


Installation & Running
----------------------
1. Extract the downloaded zip file
2. Run the wso2server.sh or wso2server.bat file in the bin directory
3. Once the server starts, point your Web browser to
   https://localhost:9443/carbon/

Hardware Requirements
-------------------
1. Minimum memory - 256MB
2. Processor      - Pentium 800MHz or equivalent at minimum

Software Requirements
-------------------
1. Java SE Development Kit - 1.6 (1.6.0_24 onwards)
2. Apache Ant - An Apache Ant version is required. Ant 1.7.0 version is recommended. 
3. The Management Console requires full Javascript enablement of the Web browser
   NOTE:
     On Windows Server 2003, it is not allowed to go below the medium security
     level in Internet Explorer 6.x.

For more details see
http://wso2.org/wiki/display/carbon/System+Requirements

Known Issues
------------

All known issues have been recorded at https://wso2.org/jira/browse/CARBON

Carbon Binary Distribution Directory Structure
--------------------------------------------

    CARBON_HOME
        |- bin <directory>
        |- dbscripts <directory>
        |- lib <directory>
        |- repository <directory>
        |--- conf <directory>
        |--- database <directory>
        |--- logs <directory>
        |--- resources <directory>
        |- tmp <directory>
        |-- LICENSE.txt <file>
        |-- README.txt <file>
        |-- INSTALL.txt <file>
        |-- release-notes.html <file>

    - bin
      Contains various scripts .sh & .bat scripts

    - conf
      Contains configuration files

    - database
      Contains the WSO2 Registry & User Manager database

    - dbscripts
      Contains the database creation & seed data population SQL scripts for
      various supported databases

    - lib
      Contains the basic set of libraries required to startup Carbon
      in standalone mode

    - logs
      Contains all log files created during execution

    - repository
      The repository where Carbon artifacts &
      Axis2 services and modules deployed in WSO2 Carbon
      are stored. In addition to this other custom deployers such as
      dataservices and axis1services are also stored.

    - tmp
      Used for storing temporary files, and is pointed to by the
      java.io.tmpdir System property

    - LICENSE.txt
      Apache License 2.0 under which WSO2 Carbon is distributed.

    - README.txt
      This document.

    - INSTALL.txt
          This document will contain information on installing WSO2 Carbon

    - release-notes.html
      Release information for WSO2 Carbon 3.2.0

Support
-------

WSO2 Inc. offers a variety of development and production support
programs, ranging from Web-based support up through normal business
hours, to premium 24x7 phone support.

For additional support information please refer to http://wso2.com/support/

For more information on WSO2 Carbon, visit the WSO2 Oxygen Tank (http://wso2.org)

Crypto Notice
-------------

This distribution includes cryptographic software.  The country in
which you currently reside may have restrictions on the import,
possession, use, and/or re-export to another country, of
encryption software.  Before using any encryption software, please
check your country's laws, regulations and policies concerning the
import, possession, or use, and re-export of encryption software, to
see if this is permitted.  See <http://www.wassenaar.org/> for more
information.

The U.S. Government Department of Commerce, Bureau of Industry and
Security (BIS), has classified this software as Export Commodity
Control Number (ECCN) 5D002.C.1, which includes information security
software using or performing cryptographic functions with asymmetric
algorithms.  The form and manner of this Apache Software Foundation
distribution makes it eligible for export under the License Exception
ENC Technology Software Unrestricted (TSU) exception (see the BIS
Export Administration Regulations, Section 740.13) for both object
code and source code.

The following provides more details on the included cryptographic
software:

Apacge Rampart   : http://ws.apache.org/rampart/
Apache WSS4J     : http://ws.apache.org/wss4j/
Apache Santuario : http://santuario.apache.org/
Bouncycastle     : http://www.bouncycastle.org/

---------------------------------------------------------------------------
(c) Copyright 2011 WSO2 Inc.

