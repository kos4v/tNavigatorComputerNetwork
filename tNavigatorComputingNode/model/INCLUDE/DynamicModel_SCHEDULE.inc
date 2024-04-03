RPTSCHED
'WELLS=2' 'SUMMARY=2' 'fip=3' 'RESTART=1' 'WELSPECS' 'CPU=2' /

INCLUDE
'INCLUDE/DynamicModel_WELLTRACK.inc' /

WELSPECS
   'Well-20-22'   1*  2*  /
/

APPLYSCRIPT
 './INCLUDE/PythonActions/script.py' 'start' 5* 7 JAN 2024  / END /
 /
/


DATES
 6 JAN 2024  /
 /



COMPDATMD
	'Well-20-22'	1*	2400	2450	MD	OPEN	2*	0.114	/
/



WCONPROD
	Well-20-22	*	BHP	*	*	*	200	*	200	/
/



DATES
 7 JAN 2024  /
 /



DATES
 8 JAN 2024  /
 /



DATES
 9 JAN 2024  /
 /



DATES
 10 JAN 2024  /
 /



DATES
 11 JAN 2024  /
 /



RPTMAPD
11 JAN 2024  /
/



DATES
 12 JAN 2024  /
 /



DATES
 13 JAN 2024  /
 /



DATES
 14 JAN 2024  /
 /



DATES
 15 JAN 2024  /
 /



DATES
 16 JAN 2024  /
 /



DATES
 17 JAN 2024  /
 /



DATES
 18 JAN 2024  /
 /



DATES
 19 JAN 2024  /
 /



DATES
 20 JAN 2024  /
 /



DATES
 21 JAN 2024  /
 /



RPTMAPD
21 JAN 2024  /
/



DATES
 22 JAN 2024  /
 /



DATES
 23 JAN 2024  /
 /



DATES
 24 JAN 2024  /
 /



DATES
 25 JAN 2024  /
 /



DATES
 26 JAN 2024  /
 /



DATES
 27 JAN 2024  /
 /



DATES
 28 JAN 2024  /
 /



DATES
 29 JAN 2024  /
 /



DATES
 30 JAN 2024  /
 /



DATES
 31 JAN 2024  /
 /



RPTMAPD
31 JAN 2024  /
/



DATES
 1 FEB 2024  /
 /



DATES
 2 FEB 2024  /
 /



DATES
 3 FEB 2024  /
 /



DATES
 4 FEB 2024  /
 /



DATES
 5 FEB 2024  /
 /



DATES
 6 FEB 2024  /
 /



DATES
 7 FEB 2024  /
 /



DATES
 8 FEB 2024  /
 /



DATES
 9 FEB 2024  /
 /



DATES
 10 FEB 2024  /
 /



RPTMAPD
10 FEB 2024  /
/



DATES
 11 FEB 2024  /
 /

END

