const TwinMattressValue = 'twin-mattress';
const TwinXLMattressValue = 'twinxl-mattress';
const FullMattressValue = 'full-mattress';
const QueenMattressValue = 'queen-mattress';
const KingMattressValue = 'king-mattress';
const CaliforniaKingMattressValue = 'california-king-mattress'

// ------
// onLoad
// ------
function changeMattressSize()
{
    // Get the size value from URL if it exists
    const urlParams = new URLSearchParams(window.location.search);
    const sizeValue = urlParams.get('size');
    if (sizeValue == null || !isValidSize(sizeValue)) { return; }

    // Find the matching option based on size above
    var textToFind = getConvertedSize(sizeValue);
    var mattressSizeSelect = document.getElementsByClassName("mattress-size");
    if (mattressSizeSelect.length != 1) { return; }

    for (var i = 0; i < mattressSizeSelect[0].options.length; i++) {
        if (mattressSizeSelect[0].options[i].text === textToFind ||
            mattressSizeSelect[0].options[i].text === `${textToFind}-Flexhead`)
        {
          mattressSizeSelect[0].selectedIndex = i;
          mattressSizeSelect[0].dispatchEvent(new Event('change'));
          break;
        }
    }
}

function getConvertedSize(sizeValue) {
  switch (sizeValue) {
      case TwinXLMattressValue:
          return "TwinXL";
      case CaliforniaKingMattressValue:
              return "California King";
      default:
          return `${sizeValue.toLowerCase().charAt(0).toUpperCase()}${sizeValue.toLowerCase().substring(1)}`
                 .replace('-mattress', '');
  }
}

function isValidSize(size)
{
    const validSizes = [
      TwinMattressValue,
      TwinXLMattressValue,
      FullMattressValue,
      QueenMattressValue,
      KingMattressValue,
      CaliforniaKingMattressValue
    ];
    return validSizes.includes(size.toLowerCase());
}

function changeMattressBase()
{
    // Get the base value from URL if it exists
    const urlParams = new URLSearchParams(window.location.search);
    const baseSlug = urlParams.get('base');
    if (baseSlug == null) { return; }

    // Find the matching options based on base above (must check all bases)
    var mattressBaseSelects = document.getElementsByClassName("mattress-base");
    if (mattressBaseSelects.length <= 0) { return; }

    for (var i = 0; i < mattressBaseSelects.length; i++)
    {
        for (var j = 0; j < mattressBaseSelects[i].options.length; j++) {
            var slug = convertBaseToSlug(mattressBaseSelects[i].options[j].text);
            if (slug === baseSlug) {
                mattressBaseSelects[i].selectedIndex = j;
                mattressBaseSelects[i].dispatchEvent(new Event('change'));
                break;
            }
        }
    }
}

function convertBaseToSlug(baseValue) {
    const result = baseValue.split('[')[0].trim().toLowerCase().replace(/[^\w ]+/g,'').replace(/ +/g,'-');
    return result;
}

changeMattressSize();
changeMattressBase();

// --------
// onChange
// --------
function updateSizeUrl(selectedSize) {
  const url = new URL(window.location);
  const key = "size";

  switch (selectedSize) {
    case 'Twin':
      url.searchParams.set(key, TwinMattressValue);
      break;
    case 'TwinXL':
      url.searchParams.set(key, TwinXLMattressValue);
      break;
    case 'Full':
      url.searchParams.set(key, FullMattressValue);
      break;
    case 'Queen':
    case 'Queen-Flexhead':
      url.searchParams.set(key, QueenMattressValue);
      break;
    case 'King':
    case 'King-Flexhead':
      url.searchParams.set(key, KingMattressValue);
      break;
    case 'California King':
    case 'California King-Flexhead':
      url.searchParams.set(key, CaliforniaKingMattressValue);
      break;
    default:
      throw new Error('Unable to match mattress size, cannot update URL.');
  }
  
  url.searchParams.delete("base");
  window.history.replaceState({}, '', url);
  ResetOtherDropdowns();
}

function getElementsByXPath(xpath) {
  let results = [];
  let query = document.evaluate(xpath, document,
      null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE, null);
  for (let i = 0, length = query.snapshotLength; i < length; ++i) {
      results.push(query.snapshotItem(i));
  }
  return results;
}

function ResetOtherDropdowns() {
  var xpath = `//option[contains(text(),'---')]`;
  var matchingElements = getElementsByXPath(xpath);
  if (matchingElements.length === 0) { return; }

  // Change the values and kick off change events
  matchingElements.forEach(element => {
      element.parentNode.value = element.value;
      // TODO: Delete after 2022.07.15
      // this was calling change events, I don't think it's necessary
      // element.parentNode.dispatchEvent(new Event('change'));
  });
}

function updateBaseUrl(selectedBase) {
  const url = new URL(window.location);
  const key = "base";

  if (selectedBase === "---") {
    url.searchParams.delete(key);
    window.history.replaceState({}, '', url);
    return;
  }
  
  url.searchParams.set(key, convertBaseToSlug(selectedBase));
  
  window.history.replaceState({}, '', url);
}

// Adds event listeners to changes for sizes and bases
var mattressSizeSelect = document.getElementsByClassName("mattress-size");
if (mattressSizeSelect.length == 1) {
  mattressSizeSelect[0].addEventListener("change", function() {
    updateSizeUrl(mattressSizeSelect[0].selectedOptions[0].label);
  });
}

var mattressBaseSelects = document.getElementsByClassName("mattress-base");
if (mattressBaseSelects.length > 0) {
  for (var i = 0; i < mattressBaseSelects.length; i++) {
    mattressBaseSelects[i].addEventListener("change", function() {
      updateBaseUrl(this.selectedOptions[0].label);
    });
  }
}