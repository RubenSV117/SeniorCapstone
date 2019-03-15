
// // Create and Deploy Your First Cloud Functions
// // https://firebase.google.com/docs/functions/write-firebase-functions
//
// exports.helloWorld = functions.https.onRequest((request, response) => {
//  response.send("Hello from Firebase!");
// });

const functions = require('firebase-functions');
const _ = require('lodash');
const request = require('request-promise');

exports.indexRecipesToElastic = functions.database.ref('regen/recipes/{ID}')
	.onWrite(event => {
		let recData = event.data.val();
		let RID   = event.params.I;

		console.log('Indexing Recipe', RID, recData);
        
		let elasticSearchConfig = functions.config().elasticsearch;
		let elasticSearchUrl = elasticSearchConfig.url + '/regen/recipes/' + RID;
		let elasticSearchMethod = recData ? 'POST' : 'DELETE';

		let elasticsearchRequest = {
			method: elasticSearchMethod,
			uri: elasticSearchUrl,
			auth: {
				username: elasticSearchConfig.username,
				password: elasticSearchConfig.password,
            },
            body: {
                name: recData.Name.toLowerCase(),
                ingredients: Object.keys(recData.Ingredients).map(function (key) { recData.Ingredients[key].IngredientName.toLowerCase()}),
                tags: Object.keys(recData.Tags).map(function (key) { recData.Tags[key].toLowerCase() }),
            },
			json: true
		};

		return request(elasticsearchRequest).then(response => {
            console.log('Elasticsearch response', response);
            return true;
		})

	});
