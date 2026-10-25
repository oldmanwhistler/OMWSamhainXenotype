# Samhain Xenotype

Small creepy nighttime xenotype perfect for carving raiders on warm Septober nights. 

## What You Get

- Xenotype: Samhain.
- Gene: Samhain.
- Gene: Sweet Tooth which enables dessert need.
- Gene: Costumed which enables anonymity need.
- Costumes: Samhain joiners wear costumes.
- Psychic Ritual: Blood and Chocolate which will summon a Samhain to join your colony.
- Scenario: Three Samhain chasing down an Ancient who doesn't celebrate the Septober harvest!

## Inspiration

Inspired by [Sam: The Soul of Halloween](https://samthesoulofhalloween.com/) created and owned by filmmaker Michael Dougherty. Sam was created in 1996 and can be seen in the 2007 movie "Trick R Treat" as well as several short films on YouTube and his website.

## Samhain Are Just Fun Little Guys

They definitely aren't child-sized murder demons. GIF from Sam's website.

![SamCarving.gif](https://samthesoulofhalloween.com/images/globals/dancers/samLoop-carving-02.gif)

## Minimum Requirements

- Biotech DLC
- [Alpha Genes](https://steamcommunity.com/sharedfiles/filedetails/?id=2891845502)
- [Big & Small: Genes and More](https://steamcommunity.com/sharedfiles/filedetails/?id=2920751126)

## Additional Content

These aren't required but they unlock additional content.

- Anomaly DLC (for Blood and Chocolate ritual)
- [Vanilla Memes Expanded](https://steamcommunity.com/sharedfiles/filedetails/?id=2636329500) (for Anonymous need with Costumed gene)
- [Vanilla Cooking Expanded](https://steamcommunity.com/sharedfiles/filedetails/?id=2134308519) (for Dessert need with Sweet Tooth gene)

## Technical Details

- Xenotype is XML-only by combining genes from Alpha Genes + Big and Small
- C# for the anomaly physic ritual and to expose needs from VME/VCE
- C# to add custom parts for the scenario

### Adding costumes

The Samhain spawn in costumes (because of course they do). Adding costumes is a simple XML patch. I included support for a variety of costumes, but you can request specific ones in the comments.

```xml
<Operation Class="PatchOperationAdd">
  <xpath>/Defs/SamhainCostumePoolDef[defName="OMW_SamhainCostumePool"]/adultCostumes</xpath>
  <value>
    <li>
      <apparel>
        <li>YourCostumeBody</li>
        <li>YourCostumeHeadwear</li>
      </apparel>
    </li>
  </value>
</Operation>
```

## Language Support

- English

## Links

- [Changelog](https://github.com/oldmanwhistler/OMWSamTrickOrTreatCostume/blob/main/CHANGELOG.md)
- [Steam](https://steamcommunity.com/sharedfiles/itemedittext/?id=3827335380)
- [GitHub](https://github.com/oldmanwhistler/OMWSamTrickOrTreatCostume)
- [AI Disclosure](https://github.com/oldmanwhistler/OMWSamTrickOrTreatCostume/blob/main/Docs/AI.md)
- [MIT License](https://github.com/oldmanwhistler/OMWSamTrickOrTreatCostume/blob/main/LICENSE)

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/oldmanwhistler/OMWStoragebornXenotype/blob/main/LICENSE) for details.

## DISCLAIMER

This is an unofficial, non-commercial fan work, not affiliated with, endorsed by, or sponsored by Michael Dougherty or Sam: The Soul of Halloween. It does not include any assets or intellectual propery from Michael Dougherty, Trick 'R Treat, etc.
