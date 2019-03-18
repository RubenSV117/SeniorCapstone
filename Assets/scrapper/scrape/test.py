from recipe_scrapers import scrape_me

# give the url as a string, it can be url from any site listed below
scraper = scrape_me('https://www.foodnetwork.com/recipes/sunny-anderson/easy-chicken-pot-pie-recipe-1923875')

print("Title: ", scraper.title())
print("Total Time: ", scraper.total_time())
print("Ingredients: ", scraper.ingredients())
#commands
scraper.title()
scraper.total_time()
scraper.ingredients()
scraper.instructions()
scraper.links()