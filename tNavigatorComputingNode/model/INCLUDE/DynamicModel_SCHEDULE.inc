-- Этот файл был сгенерирован тНавигатор v23.3-4280-ged8ebf643115.
-- Copyright (C) Рок Флоу Динамикс 2005-2023.
-- Все права защищены.

RPTSCHED
'WELLS=2' 'SUMMARY=2' 'fip=3' 'RESTART=1' 'WELSPECS' 'CPU=2' /

INCLUDE
'INCLUDE/DynamicModel_WELLTRACK.inc' /


WELSPECS
-- wname gname iw jw ref.depth phase radius inflow_flag shut-in_flag crossflow_ability_flag PVT_n density_type filt.tbl reserved reserved well_model_type polymer_mixing_tbl.n
   '2'   1*    2*    /
/

COMPDATMD
-- wname|stage_name branch|stage_num mdl|delta1 mdu|delta2 md_type status filt.tbl pi diameter Kh skin D-factor cf_mult DP completion
   '2'              1*               500        2300       MD      OPEN   2*          0.114    /
/

COMPORD
-- wname sort
   '2'   INPUT /
/

WCONPROD
-- wname status control orate wrate grate lrate lrate(rc) BHP THP VFP_n ALQ wgrate mlrate steam sat.pressure sat.temperature calorific_rate linearly_combined_rate NGL_rate
   '2'   OPEN   BHP     5*                                50  /
/

DATES
 01 FEB 2024 /
 /



DATES
 01 MAR 2024 /
 /



DATES
 01 APR 2024 /
 /



DATES
 01 MAY 2024 /
 /



DATES
 01 JUN 2024 /
 /



DATES
 01 JUL 2024 /
 /



DATES
 01 AUG 2024 /
 /



DATES
 01 SEP 2024 /
 /



DATES
 01 OCT 2024 /
 /



DATES
 01 NOV 2024 /
 /



DATES
 01 DEC 2024 /
 /



DATES
 01 JAN 2025 /
 /


END
