import os
from random import randrange

# c# скрипт заменит эту строку на необходимый путь
root_result_dir = ''

debit_dir = 'DebitData'

def re_create_dir(dir_name) -> str:
	dir_full_path = os.path.join(root_result_dir, dir_name)
	
	if not os.path.isdir(dir_full_path):
		os.mkdir(dir_full_path)
	return dir_full_path


def save_liquid_debit_by_perforation():
	dir_path = re_create_dir(debit_dir)

	df = cvpr.to_dataframe()

	file_path = os.path.join(dir_path, f'{int(get_calculated_time())}.csv')
	df.to_csv(file_path, sep=';')


def hello():
	re_create_dir(root_result_dir)
	
	save_liquid_debit_by_perforation()