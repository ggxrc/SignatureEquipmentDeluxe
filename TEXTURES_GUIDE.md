# TEXTURAS NECESSÁRIAS PARA O MOD

## ⚠️ ATENÇÃO
O mod precisa de texturas para funcionar completamente. Você pode criar suas próprias ou usar placeholders temporários.

## 📋 Lista de Texturas Necessárias

### 1. Ícone do Mod
**Arquivo**: `icon.png`
**Localização**: Raiz do mod (junto com build.txt)
**Tamanho**: 80x80 pixels
**Descrição**: Ícone que aparece na lista de mods
**Sugestão**: Uma estrela dourada ou um livro brilhante

### 2. Tomo de Gerenciamento de Assinaturas
**Arquivo**: `Content/Items/SignatureManagementTome.png`
**Tamanho**: 40x40 pixels (ou menor, será escalado)
**Descrição**: Livro mágico dourado
**Sugestão**: Um grimório com uma estrela na capa

### 3. Impulsionador de XP
**Arquivo**: `Content/Items/XPBooster.png`
**Tamanho**: 20x20 pixels (ou menor)
**Descrição**: Poção ou cristal de XP
**Sugestão**: Um frasco brilhante ou cristal azul/dourado

## 🎨 Como Criar Texturas Simples (Placeholders)

### Opção 1: Usar Paint/GIMP/Photoshop
1. Crie uma nova imagem com as dimensões especificadas
2. Pinte um desenho simples (pode ser abstrato)
3. Salve como PNG com transparência
4. Coloque na pasta correta

### Opção 2: Extrair de Texturas Existentes
1. Vá até: `C:\Program Files (x86)\Steam\steamapps\common\tModLoader\Content\Images\Item_*.png`
2. Copie uma textura similar ao que você quer
3. Modifique as cores
4. Salve com o nome correto na pasta correta

### Opção 3: Usar Geradores Online
- Piskel (https://www.piskelapp.com/) - Editor de pixel art
- Pixilart (https://www.pixilart.com/) - Outra opção de pixel art
- GIMP (gratuito) - Editor de imagens completo

## 📁 Estrutura de Pastas para Texturas

```
SignatureEquipmentDeluxe/
├── icon.png (80x80)
├── Content/
│   └── Items/
│       ├── SignatureManagementTome.png (40x40)
│       └── XPBooster.png (20x20)
```

## ⚡ Placeholders Rápidos

Se você quiser testar o mod SEM criar texturas próprias:

### icon.png (80x80)
- Baixe qualquer ícone de livro ou estrela de sites como flaticon.com
- Redimensione para 80x80
- Salve como icon.png na raiz

### SignatureManagementTome.png (40x40)
- Use a textura do Book vanilla: Item_149.png
- Recolorir para dourado
- Salvar como SignatureManagementTome.png

### XPBooster.png (20x20)
- Use a textura de Lesser Healing Potion: Item_28.png
- Recolorir para azul brilhante ou dourado
- Salvar como XPBooster.png

## 🎯 Dicas de Design

### Para o Ícone do Mod (icon.png)
- Use cores vibrantes que chamem atenção
- Dourado/amarelo são boas escolhas (tema assinatura)
- Inclua um símbolo reconhecível (estrela, livro, espada)

### Para SignatureManagementTome
- Aparência de livro ou grimório
- Cores: dourado, roxo, azul místico
- Pode ter brilho ou partículas sugeridas

### Para XPBooster
- Aparência de poção mágica ou cristal
- Cores: azul brilhante, dourado, verde luminoso
- Pequeno mas reconhecível

## 🔧 Ferramentas Recomendadas

### Gratuitas
- **GIMP** - https://www.gimp.org/
- **Paint.NET** - https://www.getpaint.net/
- **Krita** - https://krita.org/
- **Piskel** - https://www.piskelapp.com/ (online, pixel art)

### Pagas
- **Aseprite** - Melhor para pixel art ($19.99)
- **Photoshop** - Profissional mas caro

## 📝 Formato das Texturas

- **Formato**: PNG com transparência (fundo transparente)
- **Profundidade**: 32-bit RGBA
- **Compressão**: PNG padrão
- **Transparência**: Sim, canal alpha

## ⚠️ O que Acontece Sem Texturas?

Se você compilar o mod SEM criar as texturas:
- O mod ainda funcionará
- Os itens aparecerão com uma textura padrão do tModLoader (quadrado colorido)
- A funcionalidade NÃO será afetada
- Você pode adicionar texturas depois e recompilar

## 🎨 Alternativa: Pedir para a Comunidade

Se você não quiser criar texturas:
1. Poste no fórum do tModLoader
2. Peça no Discord do tModLoader
3. Procure por pixel artists freelancers
4. Use r/TerrariaMods no Reddit

## ✅ Checklist de Texturas

- [ ] icon.png criado (80x80)
- [ ] SignatureManagementTome.png criado (40x40)
- [ ] XPBooster.png criado (20x20)
- [ ] Todas as texturas têm fundo transparente
- [ ] Todas as texturas estão no formato PNG
- [ ] Todas as texturas estão nas pastas corretas

## 🚀 Após Criar as Texturas

1. Coloque os arquivos nas pastas corretas
2. Abra o tModLoader
3. Vá em Workshop -> Develop Mods
4. Encontre seu mod
5. Clique em "Build + Reload"
6. As texturas aparecerão automaticamente!

---

**Lembre-se**: As texturas são apenas visuais. O mod funciona perfeitamente sem elas, apenas parecerá menos polido.
