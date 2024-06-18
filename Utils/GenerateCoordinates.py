import json


if __name__ == '__main__':
    with open('Coordinates.xyz', 'r') as file:
        lines = file.read().split('\n')
        coordinates = {}
        for line in lines:
            c1 = line.split(' ')
            if len(c1) > 5:
                coordinates[f'{c1[0]}_{c1[1]}_{c1[2]}'] = {'x':float(c1[3]), 'y':float(c1[4]),'z':round(float(c1[5]),3)}

    with open('Coordinates.json', 'w+') as file:
        json.dump(coordinates, file)