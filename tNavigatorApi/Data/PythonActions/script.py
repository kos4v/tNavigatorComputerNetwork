import os
import sys
from pathlib import Path

# c# change this row
root_result_dir = ''

total_debit_dir = 'TotalDebitData'
oil_debit_dir = 'OilDebitData'
gas_debit_dir = 'GasDebitData'
water_debit_dir = 'WaterDebitData'
pressure_debit_dir = 'PressureData'

def re_create_dir(dir_name) -> str:
	dir_full_path = os.path.join(root_result_dir, dir_name)
	
	if not os.path.isdir(dir_full_path):
		os.mkdir(dir_full_path)
	return dir_full_path



def save(dir_path: str, data_object):
	try:
		if data_object is None:
			return
		
		dir_path = re_create_dir(dir_path)
		file_path = os.path.join(dir_path, f'{int(get_calculated_time())}.csv')
		df = data_object.to_dataframe()
		df.to_csv(file_path, sep=';')
	except Exception as e:
		print(e)
	
def start_record_files():
	re_create_dir(root_result_dir)
	
	# save_liquid_debit_by_perforation
	save(total_debit_dir, cvpr)
	
	# save_oil_debit_by_perforation()
	save(oil_debit_dir, copr)
	
	# save_gas_debit_by_perforation()
	save(gas_debit_dir, cgpr)
	
	# save_water_debit_by_perforation()
	save(water_debit_dir, cwpr)
	
	# save_water_debit_by_perforation()
	save(pressure_debit_dir, cbp)
	

def start():
	try:
		start_record_files()
	except Exception as e:
		print(e)
		
	
