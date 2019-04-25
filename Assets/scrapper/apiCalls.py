from firebase import firebase
import requests
import json
import firebase_admin
from firebase_admin import credentials
from firebase_admin import storage


firebase = firebase.FirebaseApplication('https://regen-66cf8.firebaseio.com/')

headers = {'X-RapidAPI-Key':'f8e1e1f8aamshfebb6a42267f808p1c14e3jsn1c9c22c2c1bb'}

r = requests.get('https://spoonacular-recipe-food-nutrition-v1.p.rapidapi.com/recipes/random?number=1', headers = headers)

data = r.json()

#-------------------------------------------------------------------
#get prep time, title, and steps
prepTime = data['recipes'][0]['readyInMinutes']
title = data['recipes'][0]['title']
steps = data['recipes'][0]['instructions']

#------------------------------------------------------------------
#get the tags
tag = []
if(data['recipes'][0]['vegetarian'] == True):
    tag.append('vegetarian')
if(data['recipes'][0]['vegan'] == True):
    tag.append('vegan')
if(data['recipes'][0]['glutenFree'] == True):
    tag.append('glutenFree')
if(data['recipes'][0]['dairyFree'] == True):
    tag.append('dairyFree')
if(data['recipes'][0]['ketogenic'] == True):
    tag.append('ketogenic')
if not tag:
    tag.append("NA")
#-----------------------------------------------------------
#split the steps 
step = steps.split('.')
step.remove("")
for j in range(5):
    for k in range(len(step)):
        step[k].strip()
 #-----------------------------------------------------------
 #get the ingredients
keys = ["IngredientAmount","IngredientName"]
ingrList = []
for i in range(len(data['recipes'][0]['extendedIngredients'])):
    ingrAmnt = data['recipes'][0]['extendedIngredients'][i]['amount']
    ingrUnit = ' ' + data['recipes'][0]['extendedIngredients'][i]['unit']
    ingr = str(ingrAmnt) + ingrUnit
    ingrName = data['recipes'][0]['extendedIngredients'][i]['name']
    ingredient = [ingr, ingrName]
    ingrList.append(dict(zip(keys,ingredient)))
    
result = firebase.post('recipes', {'Calories':'0', 
                                    'ImageReferencePath':' ',
                                    'ImageSprite':{'instanceID':'0'},
                                    'Ingredients':ingrList,
                                    'Name':title,
                                    'PrepTimeMinutes':prepTime,
                                    'Reviews':{'0':'NA'},
                                    'StarRating':'0',
                                    'Steps':step,
                                    'Tags':tag})
#-------------------------------------------------------------
#upload the image
n = result.get('name')
image_url = data['recipes'][0]['image']

cred = credentials.Certificate('regen-66cf8-firebase-adminsdk-l2nba-75e4f3c2bb.json')
firebase_admin.initialize_app(cred, {
    'storageBucket': 'regen-66cf8.appspot.com'
})
bucket = storage.bucket()

image_data = requests.get(image_url).content
blob = bucket.blob('Recipes/{}.jpg'.format(n))
blob.upload_from_string(
        image_data,
        content_type='image/jpg'
    )
IRP = 'gs://regen-66cf8.appspot.com/Recipes/{}.jpg'.format(n)
firebase.put('recipes/{}'.format(n), 'ImageReferencePath', IRP)   

