const { TextDecoder, TextEncoder, ReadableStream } = require('node:util');

Object.defineProperties(globalThis, {
  TextDecoder: { value: TextDecoder },
  TextEncoder: { value: TextEncoder },
  ReadableStream: { value: ReadableStream },
});

const express = require('express');
const axios = require('axios');
const cheerio = require('cheerio');
const cors = require('cors');
const app = express();

app.use(cors());

app.get('/api/funds', async (req, res) => {
    const fundCodes = req.query.codes.split(','); // Fon kodlarını virgülle ayırarak alın
    const fundData = [];

    try {
        for (const code of fundCodes) {
            const response = await axios.get(`https://www.tefas.gov.tr/FonAnaliz.aspx?FonKod=${code}`);
            const data = response.data;
            const fundName = extractFundName(data);
            const lastPrice = extractLastPrice(data);
            fundData.push({ FonKodu: code, FonAdi: fundName, SonFiyat: lastPrice });
        }
        res.json(fundData);
    } catch (error) {
        res.status(500).send('Error fetching data');
    }
});

function extractFundName(html) {
    const $ = cheerio.load(html);
    const fundName = $('#MainContent_FormViewMainIndicators_LabelFund').text().trim();
    return fundName || "Fon Adı Bulunamadı";
}

function extractLastPrice(html) {
    const $ = cheerio.load(html);
    const lastPrice = $('#MainContent_PanelInfo .main-indicators ul.top-list li span').first().text().trim();
    return lastPrice || "Fiyat Bulunamadı";
}

app.listen(3000, () => {
    console.log('Server is running on port 3000');
});
