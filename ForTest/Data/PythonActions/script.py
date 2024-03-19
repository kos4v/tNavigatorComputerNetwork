import os

# c# change this row
root_result_dir = ''

total_debit_dir = 'TotalDebitData'
oil_debit_dir = 'OilDebitData'
gas_debit_dir = 'GasDebitData'
water_debit_dir = 'WaterDebitData'

def re_create_dir(dir_name) -> str:
	dir_full_path = os.path.join(root_result_dir, dir_name)
	
	if not os.path.isdir(dir_full_path):
		os.mkdir(dir_full_path)
	return dir_full_path



def save(dir_path: str, df):
	dir_path = re_create_dir(dir_path)

	file_path = os.path.join(dir_path, f'{int(get_calculated_time())}.csv')
	df.to_csv(file_path, sep=';')
	
def start():
	re_create_dir(root_result_dir)
	
	# save_liquid_debit_by_perforation
	save(total_debit_dir, cvpr.to_dataframe())
	
	# save_oil_debit_by_perforation()
	save(oil_debit_dir, copr.to_dataframe())
	
	# save_gas_debit_by_perforation()
	save(gas_debit_dir, cgpr.to_dataframe())
	
	# save_water_debit_by_perforation()
	save(water_debit_dir, cwpr.to_dataframe())
	
	
def hello():
	try:
		start
	except  Exception as e:
		print(e)
		
	
