# Скрипт Python для генерации C# CalendarTaskInfo из tasks.js
import re
import json

# Функция для чтения и парсинга tasks.js
def parse_tasks_js(js_path):
    with open(js_path, 'r', encoding='utf-8') as f:
        content = f.read()
    # Найти объект tasks = { ... } или list внутри
    match = re.search(r'const\s+tasks\s*=\s*\{(.+)\};', content, re.DOTALL)
    if not match:
        raise ValueError('Не удалось найти блок tasks в файле')
    body = match.group(1)
    # Разбить по запятым между записями: ключ: { ... }
    entries = re.findall(r'(\d+)\s*:\s*\{([^}]*)\}', body, re.DOTALL)

    tasks = []
    for task_id, block in entries:
        # Извлечь данные
        bot_id = re.search(r'id\s*:\s*(\d+)', block).group(1)
        bot_name = re.search(r'name\s*:\s*"([^"]+)"', block).group(1)
        title = re.search(r'title\s*:\s*"([^"]+)"', block).group(1)
        description = re.search(r'description\s*:\s*"([^"]+)"', block).group(1)
        quest = re.search(r'questType\s*:\s*"([^"]+)"', block).group(1)
        # По умолчанию RequirementCount = 1, RewardType=None, RewardCount=0
        tasks.append({
            'TaskId': task_id,
            'BotId': bot_id,
            'BotName': bot_name,
            'Title': title,
            'Description': description,
            'QuestType': quest,
            'RequirementCount': 1,
            'RewardType': 'CalendarRewardType.None',
            'RewardCount': 0
        })
    return tasks

# Функция генерации C# кода
def generate_csharp(tasks):
    template = (
        '    new CalendarTaskInfo()\n'
        '    {{\n'
        '        TaskId           = {TaskId},\n'
        '        BotId            = {BotId},\n'
        '        BotName          = "{BotName}",\n'
        '        Title            = "{Title}",\n'
        '        Description      = "{Description}",\n'
        '        QuestType        = "{QuestType}",\n'
        '        RequirementCount = {RequirementCount},\n'
        '        RewardType       = {RewardType},\n'
        '        RewardCount      = {RewardCount}\n'
        '    }},'
    )
    lines = ['// Generated C# CalendarTaskInfo entries']
    for t in tasks:
        lines.append(template.format(**t))
    return '\n'.join(lines)

if __name__ == '__main__':
    js_file = 'tasks.js'  # путь к вашему файлу
    tasks = parse_tasks_js(js_file)
    csharp = generate_csharp(tasks)
    print(csharp)
